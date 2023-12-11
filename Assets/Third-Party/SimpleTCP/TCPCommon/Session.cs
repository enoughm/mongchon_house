using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;
using UnityEngine;

public abstract class Session
{
    //임의 추가한 코드 (서버 연결 여부 확인용)
    public bool IsConnected
    {
        get => _disconnected == 0;
        set => _disconnected = value ? 0 : 1;
    }
    private int _disconnected = 1;
    private Socket _socket;

    //나중에 크기는 키워야함;
    //한번에 받을 수 있는 버퍼의 크기
    private RecvBuffer _recvBuffer = new RecvBuffer(1024*100);

    private object _lock = new object();
    private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

    public abstract void OnConnected(EndPoint endPoint);
    public abstract int OnRecv(ArraySegment<byte> buffer);
    public abstract void OnSend(int numOfBytes);
    public abstract void OnDisconnected(EndPoint endPoint);

    void Clear()
    {
        lock (_lock)
        {
            _sendQueue.Clear();
            _pendingList.Clear();
        }
    }

    public void Start(Socket socket)
    {
        _socket = socket;
        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
        _recvArgs.SetBuffer(new byte[1024], 0, 1024);
        
        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        
        RegisterRecv();
    }
    
    public void Send(List<ArraySegment<byte>> sendBuffList) {
        if (sendBuffList.Count == 0)
            return;

        lock (_lock) {
            foreach (ArraySegment<byte> sendBuff in sendBuffList) {
                _sendQueue.Enqueue(sendBuff);
            }

            if (_pendingList.Count == 0) {
                RegisterSend();
            }
        }
    }


    public void Send(ArraySegment<byte> sendBuff)
    {
        lock (_lock)
        {
            _sendQueue.Enqueue(sendBuff);
            if (_pendingList.Count == 0)
                RegisterSend();
        }
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        OnDisconnected(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();

        Clear();
    }

    #region 네트워크 통신

    void RegisterSend()
    {
        if (_disconnected == 1)
            return;
            
        
        while (_sendQueue.Count > 0)
        {
            ArraySegment<byte> buff = _sendQueue.Dequeue();
            _pendingList.Add( buff);
        }
        _sendArgs.BufferList = _pendingList;

        try
        {
            bool pending = _socket.SendAsync(_sendArgs);
            if(!pending)
                OnSendCompleted(null, _sendArgs);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
    {
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    OnSend(_sendArgs.BytesTransferred);
                    
                    if (_sendQueue.Count > 0)
                        RegisterSend();
                    
                }
                catch (Exception e)
                {
                    Debug.LogError($"OnSendCompleted Failed: {e.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }
    }


    void RegisterRecv()
    {
        if (_disconnected == 1)
            return;
        
        _recvBuffer.Clean();
        ArraySegment<byte> segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

        try
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if(!pending)
                OnRecvCompleted(null, _recvArgs);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}");
        }
    }

    void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                //write cursor 이동
                if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                {
                    Disconnect();
                    return;                    
                }
                
                //컨텐츠 쪽으로 데이터 넘기고 얼마나 처리했는지 확인
                int processLen = OnRecv(_recvBuffer.ReadSegment);
                if (processLen < 0 || _recvBuffer.DataSize < processLen)
                {
                    Disconnect();
                    return;
                }

                //Read 커서 이동
                if (_recvBuffer.OnRead(processLen) == false)
                {
                    Disconnect();
                    return;
                }

                RegisterRecv();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        else
        {
            Disconnect();
        }
    }
    #endregion
}
