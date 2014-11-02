using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunCmd.Common.Messaging
{
    /// <summary>
    /// Contract for EventAggregator
    /// </summary>
    public interface IEventAggregator
    {
        //Any class should be able toSubscribe to an event type
        void Subscribe<T>(Action<T> handler);
        //Any class should be able to Unsubscribe from an event type
        void Unsubscribe<T>(Action<T> handler);
        //Any class should be able to Publish an event type
        void Publish<T>(T evt);
    }
}
