using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunCmd.Common.Messaging
{
    /// <summary>
    /// Used to Publish/Subscribe a Navigation event
    /// </summary>
    public class NavMessage
    {
        private string Notification;

        public string PageName
        {
            get { return this.Notification; }
        }


        public Dictionary<string, string> QueryStringParams { get; private set; }
        public object NavigationStateParams { get; private set; }
        public object ViewObject { get; private set; }

        public NavMessage(string pageName)
        {
            this.Notification = pageName;
        }

        public NavMessage(string pageName, Dictionary<string, string> queryStringParams)
            : this(pageName)
        {
            QueryStringParams = queryStringParams;
        }

        //Pass the instance of the View class and the ViewModel
        public NavMessage(object viewObject, object navigationStateParams)
            : this(viewObject.GetType().Name)
        {
            ViewObject = viewObject;
            NavigationStateParams=navigationStateParams;
        }
    }

    public class ObjMessage
    {
        public string Notification { get; private set; }
        public object PayLoad { get; private set; }

        public ObjMessage(string pageName, object payLoad)
        {
            Notification = pageName;
            PayLoad = payLoad;
        }
    }
}
