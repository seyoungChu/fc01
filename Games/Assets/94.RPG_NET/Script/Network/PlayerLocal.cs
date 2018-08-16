using UnityEngine;
using System.Collections;

public class PlayerLocal : MonoBehaviour {
	
	private Vector3 p;
	private Quaternion r;
	private int m;
	
	// Use this for initialization
	void Awake () {
		GetComponent<NetworkView>().observed = this;
	}
	
	void OnSerializeNetworkView(BitStream stream,NetworkMessageInfo info)
	{
		
		if(stream.isWriting)
		{
			p = gameObject.transform.position;
			r = gameObject.transform.rotation;
			m = 0;
			
			stream.Serialize(ref p);
			stream.Serialize(ref r);
			stream.Serialize(ref m);
			
		}else
		{
			Debug.Log("OnSerializeNetworkView Message PlayerLocal");
			stream.Serialize(ref p);
			stream.Serialize(ref r);
			stream.Serialize(ref m);
			
			gameObject.transform.position = new Vector3(p.x, p.y, p.z);
			gameObject.transform.rotation = new Quaternion(r.x, r.y, r.z, r.w);
			m = m;
		}
	}
	
	void OnSetCorrectionPosition( Vector3 pos ) 
	{
		
		gameObject.transform.position =  Vector3.Lerp( gameObject.transform.position , pos , 0.5f );
		
	}
	[RPC]
	void SendPosition(NetworkViewID id , Vector3 position, Quaternion rotation)
	{
		GetComponent<NetworkView>().RPC("ReceivePosition",RPCMode.OthersBuffered, GetComponent<NetworkView>().viewID, transform.position, transform.rotation);
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
}

