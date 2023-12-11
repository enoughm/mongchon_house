using System;
using System.Collections.Generic;
using System.Text;

public interface IJobQueue {
    void Push(Action job);
}
public class JobQueue : IJobQueue {
    Queue<Action> jobQueue = new Queue<Action>();
    object _lock = new object();
    bool _flush = false;

    public void Push(Action job) {
        bool flush = false;

        lock (_lock) {
            jobQueue.Enqueue(job);
            if(_flush == false) {
                flush = _flush = true;
            }
        }

        if (flush)
            Flush();
    }

    private void Flush() {
        while (true) {
            Action action = Pop();
            if (action == null)
                return;

            action.Invoke();
        }
    }

    Action Pop() {
        lock (_lock) {
            if (jobQueue.Count == 0) {
                _flush = false;
                return null;
            }
            return jobQueue.Dequeue();
        }
    }
}
