using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunCmd.Common.Messaging
{
    /// <summary>
    /// Taken from http://brentedwards.net/2010/04/13/roll-your-own-simple-message-bus-event-aggregator/
    /// </summary>
	public interface IMessageBus
	{
		void Subscribe<TMessage>(Action<TMessage> handler);
		void Unsubscribe<TMessage>(Action<TMessage> handler);
		void Publish<TMessage>(TMessage message);
		void Publish(Object message);
	}
}
