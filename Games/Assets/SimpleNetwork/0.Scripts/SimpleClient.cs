using UnityEngine;

using System.Collections;
using System.Net.Sockets;
using System.Net;

public class SimpleClient : MonoBehaviour
{

	public string m_IPAdress = "127.0.0.1";//로컬호스트 = 내 컴퓨터.
	public const int kPort = 10253;//내 연결 포트.

	private static SimpleClient singleton;


	private Socket m_Socket;
    private void Awake()
    {
        Application.runInBackground = true;
    }
	void Start()
	{
		m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


		System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(m_IPAdress);

		System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, kPort);

		singleton = this;
		try
		{
			m_Socket.Connect(remoteEndPoint);
		}
		catch (SocketException except)
		{
			Debug.LogWarning(except.Message.ToString());
		}

		Log.logInstance.ScreenLog("Connecting");
	}

	void OnApplicationQuit()
	{
		m_Socket.Close();
		m_Socket = null;
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			MessageData newmsg = new MessageData();
			newmsg.stringData = "hello";
			newmsg.mousex = Input.mousePosition.x;
			newmsg.mousey = Input.mousePosition.y;
			newmsg.type = 0;
			SimpleClient.Send(newmsg);
		}
	}

	static public void Send(MessageData msgData)
	{
		if (singleton.m_Socket == null)
		{
			return;
		}

		byte[] sendData = MessageData.ToByteArray(msgData);
		byte[] prefix = new byte[1];
		prefix[0] = (byte)sendData.Length;
		singleton.m_Socket.Send(prefix);
		singleton.m_Socket.Send(sendData);
		//Log
		Log.logInstance.ScreenLog(msgData.stringData + " " + msgData.mousex.ToString() + " " + msgData.mousey.ToString());
	}


}
