using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

    struct JobTimerElem : IComparable<JobTimerElem> {
        public int execTick; //실행 시간
        public Action action;

        public int CompareTo(JobTimerElem other) {
            return other.execTick - execTick;
        }
    }


    class JobTimer {
        PriorityQueue<JobTimerElem> pq = new PriorityQueue<JobTimerElem>();
        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0) {
            JobTimerElem job;
            job.execTick = System.Environment.TickCount + tickAfter;
            job.action = action;

            lock (_lock) {
                pq.Push(job);
            }
        }

        public void Flush() {
            while (true) {
                int now = System.Environment.TickCount;

                JobTimerElem job;

                lock (_lock) {
                    if (pq.Count == 0) {
                        break;
                    }

                    job = pq.Peek();
                    if (job.execTick > now)
                        break;

                    pq.Pop();
                }

                job.action.Invoke();
            }
        }
    }
