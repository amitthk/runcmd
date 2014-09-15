using RunCmd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunCmd
{
    public class BatFileViewModel : BaseViewModel
    {
        public BatFileViewModel()
        {
            _BatFileName = RunCmd.Common.Utility.SavedCommandsDefaultPath+"\\" +RunCmd.Common.Utility.DateTimeStampAsString+".bat";
            _cmdText = "";
        }

        private string _BatFileName;
        private string _cmdText;

        public string BatFileName
        {
            get { return _BatFileName; }
            set
            {
                if (_BatFileName != value)
                {
                    _BatFileName = value;
                    OnPropertyChanged(() => BatFileName);
                }
            }
        }

        public string CmdText
        {
            get { return _cmdText; }
            set
            {
                _cmdText = value;
                OnPropertyChanged("CmdText");
            }
        }

        public string BatFileNameDisplay
        {
            get {
                return(BatFileName.Substring(BatFileName.LastIndexOf('\\')+1));
            }
        }
    }
}
