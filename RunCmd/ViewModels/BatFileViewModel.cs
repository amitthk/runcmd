using RunCmd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunCmd
{
    public class TextFileViewModel : BaseViewModel
    {
        public TextFileViewModel()
        {
            _TextFileName = RunCmd.Common.RunCmdConstants.SavedCommandsDefaultPath+"\\" +RunCmd.Common.RunCmdConstants.DateTimeStampAsString+".bat";
            _cmdText = "";
        }

        private string _TextFileName;
        private string _cmdText;
        private bool _IsDirty;

        public bool IsDirty
        {
            get { return _IsDirty; }
        }


        public string TextFileName
        {
            get { return _TextFileName; }
            set
            {
                if (_TextFileName != value)
                {
                    _TextFileName = value;
                    _IsDirty = true;
                    OnPropertyChanged("TextFileName");
                }
            }
        }

        public string CmdText
        {
            get { return _cmdText; }
            set
            {
                if ((_cmdText != value) && (!string.IsNullOrWhiteSpace(value)))
                {
                    _cmdText = value;
                    _IsDirty = true;
                    OnPropertyChanged("CmdText");
                }
            }
        }

        public string TextFileNameDisplay
        {
            get {
                return(TextFileName.Substring(TextFileName.LastIndexOf('\\')+1));
            }
            set
            {
                TextFileName = TextFileName.Substring(0, TextFileName.LastIndexOf('\\') + 1) + value;
            }
        }
    }
}
