using System;
using System.Text;
using ServerCore;
using UnityEngine;


public enum PacketID
{
    Example = 1,
    Simple = 2,
}

public interface IPacket
{
    ushort PacketId { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}


class PacketSimple : IPacket
{
    public string packetKey = "";
    public string stringData = "";
    public byte[] bytesData = Array.Empty<byte>();

    public ushort PacketId => (ushort)PacketID.Simple;

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096 * 200);

        int count = 0;
        bool success = true;
        
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        
        //size
        count += sizeof(int); //전체 크기 데이터
        
        //packet Id
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.PacketId);
        count += sizeof(ushort);
        
        //packet key (string)
        //name 크기 추출 및 데이터 추가
        ushort packetKeyLen = (ushort)Encoding.Unicode.GetBytes(this.packetKey, 0, this.packetKey.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), packetKeyLen); //스트링 크기 입력
        count += sizeof(ushort); //string 크기
        count += packetKeyLen; //name len

        //string Data (string)
        ushort stringDataLen = (ushort)Encoding.Unicode.GetBytes(this.stringData, 0, this.stringData.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), stringDataLen); //스트링 크기 입력
        count += sizeof(ushort); //string 크기
        count += stringDataLen; //name len
        
        //byte[] bytesData
        int bytesDataLen = this.bytesData.Length;
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), bytesDataLen); //bytesData 크기 입력
        count += sizeof(int); //bytesData 크기
        Array.Copy(this.bytesData, 0, segment.Array, segment.Offset + count, bytesDataLen);
        count += bytesDataLen;
        
        //packet size는 마지막에 추가함
        success &= BitConverter.TryWriteBytes(s, count);
        
        if (!success)
            return null;
        
        return SendBufferHelper.Close(count);
    }

    public void Read(ArraySegment<byte> segment)
    {
        int count = 0;
        ushort size = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        //size
        //size =  BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(int);
        
        //packet id
        //this.packetId =  BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        
        //packet key len
        ushort packetKeyLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.packetKey = Encoding.Unicode.GetString(s.Slice(count, packetKeyLen));
        count += packetKeyLen;

        //string
        ushort stringDataLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        this.stringData = Encoding.Unicode.GetString(s.Slice(count, stringDataLen));
        count += stringDataLen;

        //bytes
        int bytesDataLen = BitConverter.ToInt32(s.Slice(count, s.Length - count));
        count += sizeof(int);
        this.bytesData = s.Slice(count, bytesDataLen).ToArray();
        count += bytesDataLen;
    }
}

class PacketExample : IPacket
{
    public long playerId;
    public string name;
    
    public ushort PacketId => (ushort)PacketID.Example;

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096 * 100);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort); //전체 크기 데이터
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), PacketId);
        count += sizeof(ushort); //packet id
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count),this.playerId);
        count += sizeof(long); //playerId
        
        //name 크기 추출 및 데이터 추가
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen); //스트링 크기 입력
        count += sizeof(ushort); //string 크기
        count += nameLen; //name len
        
        //packet size는 마지막에 추가함
        success &= BitConverter.TryWriteBytes(s, count);
        
        if (!success)
            return null;
        
        return SendBufferHelper.Close(count);
    }


    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        //id와 size의 크기만큼 count +
        //ushort size = BitConverter.ToUInt16(s.Array, s.Offset + count);
        count += sizeof(ushort);
        //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
        count += sizeof(long);
        
        //string
        ushort namelen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);

        this.name = Encoding.Unicode.GetString(s.Slice(count, namelen));
        
    }
}