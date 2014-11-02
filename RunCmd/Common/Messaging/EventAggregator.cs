using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RunCmd.Common.Messaging
{
    /// <summary>
    /// This class maintains a dictionary of Events by their "Type" and 
    /// a WeakReference to corresponding "event handlers" logged by Subscribers to that event.
    /// Any class can publish an event by loggint it here first.
    /// Any class can subscribe to events of particular type logging their subscription here.
    /// </summary>
    public sealed class EventAggregator : IEventAggregator
    {
        private Dictionary<Type, List<WeakActionRef>> _subscribers =
            new Dictionary<Type, List<WeakActionRef>>();

        private object _lock = new object();

        //Subscribe to an event type
        public void Subscribe<T>(Action<T> handler)
        {
            lock (_lock)
            {
                if (_subscribers.ContainsKey(typeof(T))) 
                {
                    //Entry for this event type exists so we add our handler to dictionary
                    var handlers = _subscribers[typeof(T)];
                    handlers.Add(new WeakActionRef(handler));
                }
                else 
                {
                    //Dictionary entry for this event type is empty so create new key and add handler to it
                    var handlers = new List<WeakActionRef>();
                    handlers.Add(new WeakActionRef(handler));
                    _subscribers[typeof(T)] = handlers;
                }
            }
        }

        //Unsubscribe from an event type
        public void Unsubscribe<T>(Action<T> handler)
        {
            lock (_lock)
            {
                if (_subscribers.ContainsKey(typeof(T)))
                {
                    var handlers = _subscribers[typeof(T)];

                    //Find out the targetReference to be removed
                    WeakActionRef targetReference = null;
                    foreach (var reference in handlers)
                    {
                        var action = (Action<T>)reference.Target;
                        if ((action.Target == handler.Target) && action.Method.Equals(handler.Method))
                        {
                            targetReference = reference;
                            break;
                        }
                    }
                    //Remove the targetReference
                    handlers.Remove(targetReference);

                    //If there are no more handlers/subscribers for this event type
                    if (handlers.Count == 0)
                    {
                        _subscribers.Remove(typeof(T));
                    }
                }
            }
        }

        //Publish an event type
        public void Publish<T>(T evt)
        {
            lock (_lock)
            {
                if (_subscribers.ContainsKey(typeof(T)))
                {
                    var handlers = _subscribers[typeof(T)];
                    foreach (var handler in handlers)
                    {
                        if (handler.IsAlive)
                        {
                            //If the handler is still alive Invoke it
                            ((Action<T>)handler.Target).Invoke(evt);
                        }
                        else
                        {
                            //Otherwise just remove the handler from dictionary
                            handlers.Remove(handler);
                        }
                    }

                    //If the number of handlers is zero, remove empty Type entry from Dictionary
                    if (handlers.Count == 0)
                    {
                        _subscribers.Remove(typeof(T));
                    }
                }
            }
        }

    }

    /// <summary>
    /// A wrapper to handler. Wraps it in WeakReference
    /// so that we can check the WeakReference every time
    /// and remove it if it isn't alive anymore
    /// </summary>
	public sealed class WeakActionRef
	{
		private WeakReference WeakReference { get; set; }

		public Delegate Target { get; private set; }

		public bool IsAlive
		{
			get { return WeakReference.IsAlive; }
		}

        //At creation maintain a weakreference to the target
		public WeakActionRef(Delegate action)
		{
			Target = action;
			WeakReference = new WeakReference(action.Target);
		}
	}
}