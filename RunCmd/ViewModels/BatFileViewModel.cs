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
            _TextFileName = RunCmd.Common.Utility.SavedCommandsDefaultPath+"\\" +RunCmd.Common.Utility.DateTimeStampAsString+".bat";
            _cmdText = "";
        }

        private string _TextFileName;
        private string _cmdText;

        public string TextFileName
        {
            get { return _TextFileName; }
            set
            {
                if (_TextFileName != value)
                {
                    _TextFileName = value;
                    OnPropertyChanged(() => TextFileName);
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
