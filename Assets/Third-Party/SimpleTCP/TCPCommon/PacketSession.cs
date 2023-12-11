using System;
using System.Text;
using ServerCore;
using UnityEngine;


public abstract class PacketSession : Session
{
    public readonly int HeaderSize = 2;
    
    //[size(2)][packetId(2)]...
    public sealed override int OnRecv(ArraySegment<byte> buffer)
    {
        int processLen = 0;
        int packetCount = 0;

        while (true)
        {
            //strDataSize와 bytesDataSize 파싱 가능여부 확인
            if (buffer.Count < HeaderSize)
                break;

            //패킷이 완전체로 도착했는지 확인
            int dataSize = BitConverter.ToInt32(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize)
                break;
            
            //여기까지 왔으면 패킷 조립 가능
            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
            packetCount++;

            processLen += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }
        
        if(packetCount > 1)
            Debug.Log($"패킷 모아보내기 : {packetCount}");
        
        return processLen;
    }

    public abstract void OnRecvPacket(ArraySegment<byte> buffer);

}