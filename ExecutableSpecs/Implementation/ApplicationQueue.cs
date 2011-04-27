using System;
using System.Collections.Concurrent;

namespace Implementation
{
    public class ApplicationQueue<T> : ConcurrentQueue<T>
    {
        private Action<T> messageHandler;

        public event EventHandler<EventArgs> OnEnqueued;
        public event EventHandler<EventArgs> OnDequeued;

        public void Clear()
        {
            lock (this)
            {
                T message;
                while (Count > 0)
                {
                    TryDequeue(out message);
                }
            }
        }

        public new void Enqueue(T message)
        {
            base.Enqueue(message);
            if (OnEnqueued != null)
            {
                OnEnqueued(this, new EventArgs());
            }
        }

        public new bool TryDequeue(out T message)
        {
            bool result = base.TryDequeue(out message);
            if (result && OnDequeued != null)
                OnDequeued(this, new EventArgs());
            return result;
        }

        public void SubscribeWithHandler(Action<T> action)
        {
            messageHandler = action;
            OnEnqueued += ((sender, args) =>
                               {
                                   T message = default(T);
                                   if (TryDequeue(out message))
                                   {
                                       action(message);
                                   }
                               });
        }
    }
}