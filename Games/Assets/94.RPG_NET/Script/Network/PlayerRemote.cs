using UnityEngine;
using System.Collections;

public class PlayerRemote : MonoBehaviour {
	
	public bool simulatePhysics = true;
	public bool updatePosition = true;
	public float physInterp = 0.1f;
	public float netInterp = 0.2f;
	public float ping;
	public float jitter;
	public GameObject localPlayer;
	public bool isResponding = false;
	public string netCode = " (No Connections)";
	private int m;
	private Vector3 p;
	private Quaternion r;
	private State[] states = new State[15];
	private int stateCount = 0;
	private Vector3 velocity;
	
	private Vector3 oriScale;
	
	
	public string ipadderss;
	
	private State nullstate;
	// Use this for initialization
	void Start () {
		GetComponent<NetworkView>().observed = this;
		nullstate = new State(Vector3.zero, Quaternion.identity, 0.0f);
		Debug.Log(nullstate.ToString());
		for(int i  = 0 ; i < states.Length; i++)
		{
			states[i] = new State(nullstate.p, nullstate.r,nullstate.t);
		}
		velocity = new Vector3(0.0f,0.0f,0.0f);
		oriScale = new Vector3(10.0f,10.0f,10.0f);//transform.localScale;
		ipadderss = GetComponent<Character_Control>().owner.ipAddress;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		GetComponent<NetworkView>().RPC("SendPosition",RPCMode.Server, GetComponent<NetworkView>().viewID, transform.position, transform.rotation);
	
	}
	[RPC]
	void SendPosition(NetworkViewID id , Vector3 position, Quaternion rotation)
	{
		
	}
	[RPC]
	void ReceivePosition(NetworkViewID id, Vector3 position, Quaternion rotation)
	{
		if(GetComponent<NetworkView>().viewID == id)
		{
			transform.position = position;
			transform.rotation = rotation;
		}
	}
	
	void FixedUpdate() {
		//Debug.Log("FixudUpdate call");
	    if(!updatePosition || states[10].Equals(nullstate)) 
		{
			//Debug.Log("state[10] is null");
			return;
		}
	   	
	    //simulatePhysics = (localPlayer && Vector3.Distance(localPlayer.transform.position, transform.position) < 30);
	    float difftime  = (float)Network.time - states[0].t;
	    jitter = Mathf.Lerp(jitter, Mathf.Abs( ping -  difftime ), Time.deltaTime * 0.3f);
	    ping = Mathf.Lerp(ping, (float)Network.time - states[0].t, Time.deltaTime * 0.3f);
	   
	    //character.rigidbody.isKinematic = !simulatePhysics;
	    //character.rigidbody.interpolation = (simulatePhysics ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
	   
	    //Interpolation
	    float interpolationTime  = (float)Network.time - netInterp;
	    if (states[0].t > interpolationTime) {                        // Target playback time should be present in the buffer
	        int i;
	        for (i=0; i<stateCount; i++) {                          // Go through buffer and find correct state to play back
	            if (states[i] != null && (states[i].t <= interpolationTime || i == stateCount-1)) {
	                State rhs = states[Mathf.Max(i-1, 0)];                            // The state one slot newer than the best playback state
	                State lhs = states[i];                                            // The best playback state (closest to .1 seconds old)
	                float l = rhs.t - lhs.t;                      // Use the time between the two slots to determine if interpolation is necessary
	                float t = 0.0f;                                                    // As the time difference gets closer to 100 ms, t gets closer to 1 - in which case rhs is used
	                if (l > 0.0001f) t = ((interpolationTime - lhs.t) / l);          // if t=0 => lhs is used directly
	                if(simulatePhysics) {
	                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, Vector3.Lerp(lhs.p, rhs.p, t), physInterp);
	                    gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Slerp(lhs.r, rhs.r, t), physInterp);
	                    velocity = ((rhs.p - states[i + 1].p) / (rhs.t - states[i + 1].t));
	                }
	                else {
	                    gameObject.transform.position = Vector3.Lerp(lhs.p, rhs.p, t);
	                    gameObject.transform.rotation = Quaternion.Slerp(lhs.r, rhs.r, t);
	                }
	                isResponding = true;
	                netCode = "";
	                return;
	            }
	        }
	    }
	   
	    //Extrapolation
	    else  {
	        float extrapolationLength  = (interpolationTime - states[0].t);
	        if (extrapolationLength < 1 && states[0].Equals(nullstate) == false && states[1].Equals(nullstate) == false ) {
	            if(!simulatePhysics) {
	                gameObject.transform.position = states[0].p + (((states[0].p - states[1].p) / (states[0].t - states[1].t)) * extrapolationLength);
	                gameObject.transform.rotation = states[0].r;
	            }
	            isResponding = true;
	            if(extrapolationLength < 0.5f) netCode = ">";
	            else netCode = " (Delayed)";
	        }
	        else {
	            netCode = " (Not Responding)";
	            isResponding = false;
	        }
	    }
	    if(simulatePhysics && states[0].t > states[2].t) 
		{
			velocity = ((states[0].p - states[2].p) / (states[0].t - states[2].t));
		}
	    
	    
	    // scale... y? 
	    if(transform.localScale != oriScale)
	    {
	    	transform.localScale = oriScale;
	    }
	    
	}
	void OnSerializeNetworkView(BitStream stream,NetworkMessageInfo info) {
	    //We are the server, and have to keep track of relaying messages between connected clients
	    if(stream.isWriting) {
			Debug.LogError("what the?!");
	        if(stateCount == 0) 
			{
				Debug.Log("stateCount == 0");
				return;
			}
			Debug.Log("States Sending ~ : " + states[0].p.ToString() + " " + states[0].r.ToString());
	        p = states[0].p;
	        r = states[0].r;
	        m = (int)(((float)Network.time - states[0].t) * 1000);    //m is the number of milliseconds that transpire between the packet's original send time and the time it is resent from the server to all the other clients
	        stream.Serialize(ref p);
	        stream.Serialize(ref r);
	        stream.Serialize(ref m);
	    }
	   
	    //New packet recieved - add it to the states array for interpolation!
	    else {
			//Debug.Log("New packet recieved - add it to the states array for interpolation!");
	        stream.Serialize(ref p);
	        stream.Serialize(ref r);
	        stream.Serialize(ref m);
	        State state = new State(p, r, (float)(info.timestamp - (m > 0 ? (m / 1000) : 0.0f)));
			//Debug.Log(state.p.ToString() +" "+ state.r.ToString() + " " + state.t.ToString());
	        if(stateCount == 0) 
			{
				states[0] = new State(state.p, state.r, state.t);//state;
				//Debug.Log("states 0 is : " + states[0].ToString());
				
			}else if(state.t > states[0].t) {
	            for (int k = states.Length-1 ; k > 0 ; k--) 
				{
					states[k] = states[k-1];
					//Debug.Log("states[k] is : " + states[k].ToString());
				}
	            states[0] = new State(state.p, state.r, state.t);//state;
				//Debug.Log("new states 0 is : " + states[0].ToString());
	        }
	        else 
	        {
	        	Debug.Log(gameObject.name + ": Out-of-order state received and ignored (" + ((states[0].t - state.t) * 1000) + ")" + states[0].t + "---" + state.t + "---" + m + "---" + states[0].p.x + "---" + state.p.x);
	        }
	        stateCount = Mathf.Min(stateCount + 1, states.Length);
	    }
	    
	    
    
	}
	
	void OnSetCorrectionPosition( Vector3 pos ) {
	
		if(states[0].ToString().Length > 0)
		{
			states[0].p = Vector3.Lerp( gameObject.transform.position , pos , 0.5f );
		}
	}
	
	public class State : System.Object {
		public Vector3 p;
		public Quaternion r;
		public float t;
		
		public State(Vector3 p , Quaternion r , float t)
		{
			this.p = p;
			this.r = r;
			this.t = t;
		}
	}
}



/*
 var simulatePhysics : boolean = true;

var updatePosition : boolean = true;
var physInterp = 0.1;
var netInterp = 0.2;
var ping : float;
var jitter : float;
var localPlayer : GameObject;      //The "Player" GameObject for which this game instance is authoritative. Used to determine if we should be calculating physics on the object this script is controlling, in case it could be colliding with this game instance's "player"
var isResponding = false;         //Updated by the script for diagnostic feedback of the status of this NetworkView
var netCode = " (No Connection)";   //Updated by the script for diagnostic feedback of the status of this NetworkView
private var m : int;
private var p : Vector3;
private var r : Quaternion;
private var states = new State[15];
private var stateCount : int;
private var velocity : Vector3;
//private var character : CharacterController;

class State extends System.Object {
    var p : Vector3;
    var r : Quaternion;
    var t : float;
    function State(p : Vector3, r : Quaternion, t : float) {
        this.p = p;
        this.r = r;
        this.t = t;
    }
}

function Start() {
//	character = GetComponent( CharacterController );
    networkView.observed = this;
}

function FixedUpdate() {
    if(!updatePosition || !states[10]) return;
   
    //simulatePhysics = (localPlayer && Vector3.Distance(localPlayer.transform.position, transform.position) < 30);
    var difftime : float = Network.time - states[0].t;
    jitter = Mathf.Lerp(jitter, Mathf.Abs( ping -  difftime ), Time.deltaTime * 0.3);
    ping = Mathf.Lerp(ping, Network.time - states[0].t, Time.deltaTime * 0.3);
   
    //character.rigidbody.isKinematic = !simulatePhysics;
    //character.rigidbody.interpolation = (simulatePhysics ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
   
    //Interpolation
    var interpolationTime : float = Network.time - netInterp;
    if (states[0].t > interpolationTime) {                        // Target playback time should be present in the buffer
        var i : int;
        for (i=0; i<stateCount; i++) {                          // Go through buffer and find correct state to play back
            if (states[i] && (states[i].t <= interpolationTime || i == stateCount-1)) {
                var rhs : State = states[Mathf.Max(i-1, 0)];                            // The state one slot newer than the best playback state
                var lhs : State = states[i];                                            // The best playback state (closest to .1 seconds old)
                var l : float = rhs.t - lhs.t;                      // Use the time between the two slots to determine if interpolation is necessary
                var t : float = 0.0;                                                    // As the time difference gets closer to 100 ms, t gets closer to 1 - in which case rhs is used
                if (l > 0.0001) t = ((interpolationTime - lhs.t) / l);          // if t=0 => lhs is used directly
                if(simulatePhysics) {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, Vector3.Lerp(lhs.p, rhs.p, t), physInterp);
                    gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Slerp(lhs.r, rhs.r, t), physInterp);
                    velocity = ((rhs.p - states[i + 1].p) / (rhs.t - states[i + 1].t));
                }
                else {
                    gameObject.transform.position = Vector3.Lerp(lhs.p, rhs.p, t);
                    gameObject.transform.rotation = Quaternion.Slerp(lhs.r, rhs.r, t);
                }
                isResponding = true;
                netCode = "";
                return;
            }
        }
    }
   
    //Extrapolation
    else  {
        var extrapolationLength : float = (interpolationTime - states[0].t);
        if (extrapolationLength < 1 && states[0] && states[1]) {
            if(!simulatePhysics) {
                gameObject.transform.position = states[0].p + (((states[0].p - states[1].p) / (states[0].t - states[1].t)) * extrapolationLength);
                gameObject.transform.rotation = states[0].r;
            }
            isResponding = true;
            if(extrapolationLength < .5) netCode = ">";
            else netCode = " (Delayed)";
        }
        else {
            netCode = " (Not Responding)";
            isResponding = false;
        }
    }
    if(simulatePhysics && states[0].t > states[2].t) velocity = ((states[0].p - states[2].p) / (states[0].t - states[2].t));
    
    
    // scale... y? 
    if(transform.localScale != Vector3.one)
    {
    	transform.localScale = Vector3.one;
    }
    
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo) {
    //We are the server, and have to keep track of relaying messages between connected clients
    if(stream.isWriting) {
        if(stateCount == 0) return;
        p = states[0].p;
        r = states[0].r;
        m = (Network.time - states[0].t) * 1000;    //m is the number of milliseconds that transpire between the packet's original send time and the time it is resent from the server to all the other clients
        stream.Serialize(p);
        stream.Serialize(r);
        stream.Serialize(m);
    }
   
    //New packet recieved - add it to the states array for interpolation!
    else {
        stream.Serialize(p);
        stream.Serialize(r);
        stream.Serialize(m);
        var state : State = new State(p, r, info.timestamp - (m > 0 ? (parseFloat(m) / 1000) : 0));
        if(stateCount == 0) states[0] = state;
        else if(state.t > states[0].t) {
            for (k=states.Length-1;k>0;k--) states[k] = states[k-1];
            states[0] = state;
        }
        else 
        {
        	Debug.Log(gameObject.name + ": Out-of-order state received and ignored (" + ((states[0].t - state.t) * 1000) + ")" + states[0].t + "---" + state.t + "---" + m + "---" + states[0].p.x + "---" + state.p.x);
        }
        stateCount = Mathf.Min(stateCount + 1, states.Length);
    }
    
    
    
}

function OnSetCorrectionPosition( pos : Vector3 ) {
	
	if(states[0])
	{
		states[0].p = Vector3.Lerp( gameObject.transform.position , pos , 0.5 );
	}
} 
*/