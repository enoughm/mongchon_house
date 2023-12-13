using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TCPClientManager : MonoBehaviour
{
    [SerializeField] private bool isLocalhost = false;
    [SerializeField] private string clientIP = "127.0.0.1"; 
    private const int Port = 7777;
    private ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
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
        if (!isLocalhost)
        {
            IPAddress ipAddr = IPAddress.Parse(clientIP);
            StartConnectionLoop(ipAddr, Port);
        }
        else
        {
            StartConnectionLoop(IPAddress.Loopback, Port);
        }
    }
    
    public void StartConnectionLoop(IPAddress address, int port = 7777)
    {
        StartCoroutine(ConnectionLoop(address, port));
    }

    IEnumerator ConnectionLoop(IPAddress address, int port = 7777)
    {
        while (true)
        {
            if (_session != null && !_session.IsConnected)
            {
                _session = new ServerSession();
                IPEndPoint endPoint = new IPEndPoint(address, port);
                Connector connector = new Connector();
                connector.Connect(endPoint, () => _session, 1);
            }

            yield return new WaitForSeconds(10f);
        }
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            ClientPacketManager.Instance.HandlePacket(_session, packet);
    }

    private void OnApplicationQuit()
    {
        _session?.Disconnect();
    }
}
