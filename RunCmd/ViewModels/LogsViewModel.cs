using RunCmd.Common;
using RunCmd.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunCmd.ViewModels
{
    public class LogsViewModel : BaseViewModel
    {
        private ObservableCollection<TextFileViewModel> _LogFiles;
        private TextFileViewModel _SelectedLogFile;
        private readonly ICommand _GoBackCmd;
        private readonly ICommand _DeleteLogCmd;
        private readonly ICommand _ViewLogCmd;
        private Common.Messaging.IEventAggregator _eventAggregator;


        public ICommand GoBackCmd { get { return (_GoBackCmd); } }
        public ICommand DeleteLogCmd { get { return (_DeleteLogCmd); } }
        public ICommand ViewLogCmd { get { return (_ViewLogCmd); } }

        public TextFileViewModel SelectedLogFile
        {
            get { return _SelectedLogFile; }
            set
            {
                if ((_SelectedLogFile != value) && (value != null))
                {
                    _SelectedLogFile = value;
                    OnPropertyChanged("SelectedLogFile");
                }
            }
        }

        public ObservableCollection<TextFileViewModel> LogFiles
        {
            get { return _LogFiles; }
            set { 
                _LogFiles = value;
                OnPropertyChanged("LogFiles");
            }
        }


        public LogsViewModel()
        {
            _eventAggregator = App.eventAggregator;
            _GoBackCmd = new RelayCommand(ExecGoBackCmd, CanGoBackCmd);
            _DeleteLogCmd = new RelayCommand(ExecDeleteLog,CanDeleteLog);
            _ViewLogCmd = new RelayCommand(ExecViewLogCmd, CanViewLogCmd);
            _LogFiles = LoadLogFiles();
        }

        private bool CanViewLogCmd(object obj)
        {
            return (SelectedLogFile != null);
        }

        private void ExecViewLogCmd(object obj)
        {
            System.Diagnostics.Process.Start(SelectedLogFile.TextFileName);
        }

        private bool CanDeleteLog(object obj)
        {
            return (SelectedLogFile != null);
        }

        private void ExecDeleteLog(object obj)
        {
            Utility.DeleteFile(SelectedLogFile.TextFileName);
            LogFiles = LoadLogFiles();
        }

        private ObservableCollection<TextFileViewModel> LoadLogFiles()
        {
            ObservableCollection<TextFileViewModel> rtrn = null;
            string SavedLogsLoc = RunCmdConstants.DefaultLogFolder;

            if (Utility.DirExists(SavedLogsLoc))
            {
                var lstitms = Utility.GetAllFileNames(SavedLogsLoc, "*.log");

                if ((lstitms != null) && (lstitms.Length > 0))
                {
                    rtrn = new ObservableCollection<TextFileViewModel>();
                    foreach (var item in lstitms)
                    {
                        rtrn.Add(new TextFileViewModel(){ TextFileName=item});
                    }
                }
            }
            return (rtrn);
        }

        private void ExecGoBackCmd(object obj)
        {
            _eventAggregator.Publish(new NavMessage("HomeView"));
        }

        private bool CanGoBackCmd(object obj)
        {
            return (true);
        }
    }
}
