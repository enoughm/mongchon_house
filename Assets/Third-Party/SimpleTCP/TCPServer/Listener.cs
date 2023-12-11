using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Listener
{
    private Socket _listenSocket;
    private Func<Session> _sessionFactory;

    public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _sessionFactory += sessionFactory;
      
        
        _listenSocket.Bind(endPoint);
        
        //backlog = 최대 대기 수
        _listenSocket.Listen(10);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
        
        RegisterAccept(args);
    }

    void RegisterAccept(SocketAsyncEventArgs args)
    {
        args.AcceptSocket = null;
        
        bool pending = _listenSocket.AcceptAsync(args);
        if (!pending)
            OnAcceptCompleted(null, args);
    }

    void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            Session session = _sessionFactory.Invoke();
            session.IsConnected = true;  //임의 추가한 코드 (서버 연결 여부 확인용)
            session.Start(args.AcceptSocket);
            session.OnConnected(args.AcceptSocket.RemoteEndPoint);
        }
        else
        {
            Debug.LogError(args.SocketError.ToString());
        }
        
        RegisterAccept(args);
    }
}