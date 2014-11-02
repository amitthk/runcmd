using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunCmd.Common;
namespace RunCmd.Common
{
    struct RunCmdConstants
    {
        public static string DefaultLogFileName { get { return (Utility.getAsolutePathForRelativeFileName("log", "Error" + DateTimeStampAsString + ".log")); } }
        public static string DefaultLogFolder { get { return (Utility.getAbsolutePathForRelativeDir("log")); } }
        public static string ExePathDefault { get { return ("Cmd.exe"); } }
        public static string SavedCommandsDefaultPath { get { return (Utility.getAbsolutePathForRelativeDir("Commands")); } }
        public static string ConfigDefaultPath { get { return (Utility.getAsolutePathForRelativeFileName("config", "AppConfig.cfg")); } }
        public static string DateTimeStampAsString { get { return (DateTime.Now.ToString("ddMMMyyyy_hhmm")); } }
    }
}
