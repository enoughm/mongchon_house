using System;
using System.Net;
using UnityEngine;


public class TCPServerManager : MonoBehaviour
{

    [SerializeField] private bool isLocalhost = false;
    public string clientIP = "127.0.0.1";
    private const int Port = 7777;
    private Listener _listener = new Listener();
    public static GameRoom Room = new GameRoom();
    
    void FlushRoom() {
        Room.Push(() => Room.Flush());
        
        //tickAfter 수정 시 패킷 처리 속도를 조절 가능합니다.
        JobTimer.Instance.Push(FlushRoom, 250);
    }

    private void Awake()
    {
        //Loom 사용은 main thread에서 서버로직을 실행하기 위해 추가했습니다.
        Loom.Initialize();
    }

    public void Start()
    {
        //localhost로 실행
        // string host = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(host);
        // IPAddress ipAddr = ipHost.AddressList[0];
        if(Application.isEditor)
            isLocalhost = true;
        
        if (!isLocalhost)
        {
            IPAddress ipAddr = IPAddress.Parse(clientIP);
            OpenServer(ipAddr, Port);
        }
        else
        {
            OpenServer(IPAddress.Loopback, Port);
        }
        //Server 실행
    }

    public void OpenServer(IPAddress ipAddress, int port)
    {
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        _listener.Init(endPoint, () => SessionManager.Instance.Generate());
        Debug.Log("Listening...");
        JobTimer.Instance.Push(FlushRoom);
    }

    private void Update()
    {
        JobTimer.Instance.Flush();
    }
    
    private void OnDestroy()
    {
        Room.DisconnectAll();
    }
}
