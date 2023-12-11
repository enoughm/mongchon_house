using System;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom
{
        private List<ClientSession> _sessions = new List<ClientSession>();
        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        private object _lock = new object();
        private JobQueue _jobQueue = new JobQueue();

        public void Push(Action job)
        {
                _jobQueue.Push(job);
        }

        public void Flush()
        {
                foreach (ClientSession s in _sessions)
                        s.Send(_pendingList);
                _pendingList.Clear();
        }

        
        public void Broadcast(ArraySegment<byte> segment)
        {
                _pendingList.Add(segment);
        }
        
        public void Enter(ClientSession session)
        {
                Debug.Log($"[Server] [Room Entered] session.SessionId:{session.SessionId}");
                _sessions.Add(session);
                session.Room = this;
        }

        public void Leave(ClientSession session)
        {
                lock (_lock)
                {
                        _sessions.Remove(session);
                }
        }
}