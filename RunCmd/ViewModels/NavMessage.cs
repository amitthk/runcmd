using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunCmd.ViewModels
{
    class NavMessage
    {
        private string _EventType;
        public string EventType { get { return (_EventType); } }

        public NavMessage(string EvenType)
        {
            // TODO: Complete member initialization
            this._EventType = EvenType;
        }
    }

    /// <summary>
    /// We can use this message to navigate to another page as used here
    /// http://stackoverflow.com/questions/3857486/where-does-the-navigation-logic-belong-view-viewmodel-or-elsewhere
    /// </summary>
    public class NavigationMessage
    {
        private string Notification;

        public string PageName
        {
            get { return this.Notification; }
        }

        public Dictionary<string, string> QueryStringParams { get; private set; }

        public NavigationMessage(string pageName) {
            this.Notification = pageName;
        }

        public NavigationMessage(string pageName, Dictionary<string, string> queryStringParams)
            : this(pageName)
        {
            QueryStringParams = queryStringParams;
        }
    }
}
