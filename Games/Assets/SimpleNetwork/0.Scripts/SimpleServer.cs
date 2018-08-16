using UnityEngine;

using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;


public class SimpleServer : MonoBehaviour
{

	static SimpleServer singleton;

	private Socket m_Socket;
    //이 서버에 연결된 클라이언트 리스트.
	ArrayList m_Connections = new ArrayList();
    //클라이언트로터 데이터를 읽어들일 리스트.
	ArrayList m_Buffer = new ArrayList();
	ArrayList m_ByteBuffer = new ArrayList();

    private void Awake()
    {
        //이 프로그램이 포커스를 잃더라도 Update를 호출시켜 주세요.
        Application.runInBackground = true;
    }

    void Start()
	{
		Log.logInstance.ScreenLog("start listening...");
        //통신용 소켓 생성.
		m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, SimpleClient.kPort);//10253

        m_Socket.Bind(ipLocal);

        //start listening...
        //클라이언트의 연결을 대기하는 상태.
        m_Socket.Listen(100);
        singleton = this;
        
	}

	void OnApplicationQuit()
	{
		Cleanup();
	}

	void Cleanup()
	{
        //소켓 종료.
		if (m_Socket != null)
		{
			m_Socket.Close();         
        }
		m_Socket = null;
        //연결된 클라이언트 소켓들도 종료.
		foreach (Socket con in m_Connections)
		{
			con.Close();         
        }
		m_Connections.Clear();
	}


	void Update()
	{
		// Accept any incoming connections!
		ArrayList listenList = new ArrayList();
		listenList.Add(m_Socket);
        //연결 요청들어온 클라이언트가 있는지 확인.
		Socket.Select(listenList, null, null, 1000);
		for (int i = 0; i < listenList.Count; i++)
		{
			Socket newSocket = ((Socket)listenList[i]).Accept();
			m_Connections.Add(newSocket);
			m_ByteBuffer.Add(new ArrayList());
			Debug.Log("Did connect");
			Log.logInstance.ScreenLog("Did connect");
		}

		// Read data from the connections!
		if (m_Connections.Count != 0)
		{
			ArrayList connections = new ArrayList(m_Connections);
			Socket.Select(connections, null, null, 1000);
			// Go through all sockets that have data incoming!
			foreach (Socket socket in connections)
			{
				byte[] receivedbytes = new byte[512];

				ArrayList buffer = (ArrayList)m_ByteBuffer[m_Connections.IndexOf(socket)];
				int read = socket.Receive(receivedbytes);
				for (int i = 0; i < read; i++)
				{
					buffer.Add(receivedbytes[i]);               
                }

				while (true && buffer.Count > 0)
				{
					int length = (byte)buffer[0];

					if (length < buffer.Count)
					{
						ArrayList thismsgBytes = new ArrayList(buffer);
						thismsgBytes.RemoveRange(length + 1, thismsgBytes.Count - (length + 1));
						thismsgBytes.RemoveRange(0, 1);
                        
						buffer.RemoveRange(0, length + 1);
						byte[] readbytes = (byte[])thismsgBytes.ToArray(typeof(byte));

						MessageData readMsg = MessageData.FromByteArray(readbytes);
						m_Buffer.Add(readMsg);

						Debug.Log(System.String.Format("Message {0}: {1}, {2}", readMsg.stringData, readMsg.mousex, readMsg.mousey));
						Log.logInstance.ScreenLog(System.String.Format("Message {0}: {1}, {2}", readMsg.stringData, readMsg.mousex, readMsg.mousey));
      
					}
					else
						break;
				}


			}
		}
	}

}
