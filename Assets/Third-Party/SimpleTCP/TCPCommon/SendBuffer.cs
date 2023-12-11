using System;
using System.Threading;

namespace ServerCore {
    public static class SendBufferHelper {
        private static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });
        
        //청크 사이즈는 reserveSize보다 커야함
        private static int ChunkSize { get; set; } = 4096 * 200;
        
        /// <param name="reserveSize">보낼 데이터의 크기보다 커야됨 (bytes)</param>
        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (ChunkSize < reserveSize)
                ChunkSize = reserveSize + ChunkSize;
            
            if (CurrentBuffer.Value == null) {
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            }

            if (CurrentBuffer.Value.FreeSize < reserveSize) {
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            }

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize) {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer {

        //[][][][][u][][][][][]
        byte[] buffer;
        int usedSize = 0;

        public int FreeSize { get { return buffer.Length - usedSize; } }

        public SendBuffer(int chunkSize) {
            buffer = new byte[chunkSize];
        }

        public ArraySegment<byte> Open(int reserveSize) {
            //if (reserveSize > FreeSize)
            //    return null;
            return new ArraySegment<byte>(buffer, usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize) {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, this.usedSize, usedSize);
            this.usedSize += usedSize;
            return segment;
        }
    }
}