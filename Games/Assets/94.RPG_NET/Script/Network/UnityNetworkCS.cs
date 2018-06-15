using UnityEngine;
using System.Collections;

public class UnityNetworkCS : MonoBehaviour {


	public string connectToIP = "127.0.0.1";

	public int connectPort = 12345;

	public string BeginStage = "stage1";

	public Transform KnightPlayerPrefab;

	public ArrayList playerScripts = new ArrayList();

	
void  Awake (){
	
	DontDestroyOnLoad(gameObject);
	Application.runInBackground = true;
		
	if(Network.peerType == NetworkPeerType.Server)
	{
		Debug.Log("Player create ready - awake");
		Spawnplayer(Network.player );
	}

}

void  OnServerInitialized (){

	Debug.Log( "Player create by server" );

	Spawnplayer(Network.player);

}

void  OnPlayerConnected ( NetworkPlayer newPlayer  ){

	Debug.Log("A player connected to me(the server)!");

	Spawnplayer(newPlayer );

}	

void  Spawnplayer ( NetworkPlayer newPlayer )
	{
		
	//Called on the server only
	int playerNumber = int.Parse(newPlayer+"");
	//Instantiate a new object for this player, remember; the server is therefore the owner.
	Transform myNewTrans = null;
	Debug.Log("SpawnPLayer called");
	
	
    if (Network.isServer)
    {
        myNewTrans = Network.Instantiate(KnightPlayerPrefab, transform.position, transform.rotation, playerNumber) as Transform;
    }
    	
	if(myNewTrans != null)
	{
		//Get the networkview of this new transform
		NetworkView newObjectsNetworkview = myNewTrans.GetComponent<NetworkView>();

		//Keep track of this new player so we can properly destroy it when required.
		playerScripts.Add(myNewTrans.GetComponent("Character_Control"));

		Debug.Log("Player Spawn :"+playerNumber);

		//Call an RPC on this new networkview, set the player who controls this player
		newObjectsNetworkview.RPC("SetPlayer", RPCMode.AllBuffered, newPlayer);//Set it on the owner
	}else
	{
		Debug.LogError("Can't Instantiated the KnightPlayerPrefab~!!");
	}

}
	
void  OnPlayerDisconnected ( NetworkPlayer player  ){
		
	Debug.Log("Clean up after player " + player);
		
	foreach(Character_Control script in playerScripts){

		if(player==script.owner){//We found the players object

			Network.RemoveRPCs(script.gameObject.GetComponent<NetworkView>().viewID);//remove the bufferd SetPlayer call

			Network.Destroy(script.gameObject);//Destroying the GO will destroy everything

			playerScripts.Remove(script);//Remove this player from the list

			break;

		}

	}
		
	//Remove the buffered RPC call for instantiate for this player.
	int playerNumber = int.Parse(player+"");
	Network.RemoveRPCs(Network.player, playerNumber);

	// The next destroys will not destroy anything since the players never
	// instantiated anything nor buffered RPCs
	Network.RemoveRPCs(player);
	Network.DestroyPlayerObjects(player);

}

void  OnDisconnectedFromServer ( NetworkDisconnection info  ){

	Debug.Log("Resetting the scene the easy way.");
	Application.LoadLevel(Application.loadedLevel);	

}

[RPC]
void  RequestSeletedCharacter (){
		
	int selected = 0;
		
	GetComponent<NetworkView>().RPC( "ResultSelectedCharacter", RPCMode.Server,selected , Network.player);

}

[RPC]
void  ResultSelectedCharacter (  int selected  ,   NetworkPlayer newPlayer  ){
		
	if( selected == 0 )
	{
		Spawnplayer(newPlayer );
	}

}
//Obviously the GUI is for both client&servers (mixed!)
void  OnGUI (){

	if (Network.peerType == NetworkPeerType.Disconnected){

		GUILayout.BeginArea( new Rect(140,64,200,256));

		//We are currently disconnected: Not a client or host
		GUILayout.Label("Connection status: Disconnected");
			
		connectToIP = GUILayout.TextField(connectToIP, GUILayout.MinWidth(100));

		connectPort = int.Parse(GUILayout.TextField(connectPort.ToString()));
		
		if (GUILayout.Button ("Connect as client",   GUILayout.MinHeight(40)))
		{
			//Connect to the "connectToIP" and "connectPort" as entered via the GUI
			//Ignore the NAT for now
			Debug.Log("Click Connect as client");
			Network.Connect(connectToIP, connectPort);
				
		}

		if (GUILayout.Button ("Start Server",   GUILayout.MinHeight(40)))
		{
			//Start a server for 32 clients using the "connectPort" given via the GUI
			//Ignore the nat for now
			Debug.Log("Click Start Server");			
			Network.InitializeServer(32, connectPort,false);

		}
		GUILayout.EndArea();

	}else{
			
		//We've got a connection(s)!
		GUILayout.BeginArea( new Rect(380,20,100,200));
			
		if (Network.peerType == NetworkPeerType.Connecting){
				
			GUILayout.Label("Connection status: Connecting");

		} else if (Network.peerType == NetworkPeerType.Client){

			GUILayout.Label("Connection status: Client!");
			GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );
			//Application.LoadLevelAdditive(BeginStage);
			//Application.LoadLevel(BeginStage);		

		} else if (Network.peerType == NetworkPeerType.Server){

			GUILayout.Label("Connection status: Server!");
			GUILayout.Label("Connections: "+Network.connections.Length);

			if(Network.connections.Length>=1){

				GUILayout.Label("Ping fp: "+Network.GetAveragePing(  Network.connections[0] ) );

			}
			//Debug.Log("connect server");
			//Application.LoadLevel(BeginStage);			
			//Application.LoadLevelAdditive(BeginStage);

		}

		if (GUILayout.Button ("Disconnect"))
		{
			Debug.Log("click DisConnect");
			Network.Disconnect(200);
		}

		GUILayout.EndArea();
	}

}
}


