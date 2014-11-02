using RunCmd.Common;
using RunCmd.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunCmd.ViewModels
{
    public class OptionsWindowViewModel :BaseViewModel
    {
        private readonly ICommand _ChangeExePathCmd;
        private readonly ICommand _ChangeSavedCommandsLocCmd;
        private readonly ICommand _ResetSettingsCmd;

        public ICommand ResetSettingsCmd { get { return (_ResetSettingsCmd); } }


        public ICommand ChangeExePathCmd { get { return (_ChangeExePathCmd); } }
        public ICommand ChangeSavedCommandsLocCmd { get { return (_ChangeSavedCommandsLocCmd); } }
        
        private readonly IEventAggregator _eventAggregator;

        private string _ExePath;
        private string _SavedCommandsPath;
        private bool _SaveLogFiles;
        private bool _ConfirmBeforeRun;

        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator; }
        }

        public OptionsWindowViewModel()
        {
            //Dirty instantiate IEventAggregator = shoud use Dependency Injection in Real App
            _eventAggregator = App.eventAggregator;
            _ExePath = Settings.Instance.ExePath;
            _SavedCommandsPath = Settings.Instance.SavedCommandsPath;
            _SaveLogFiles = Settings.Instance.IsSaveLogsEnabled;
            _ConfirmBeforeRun = Settings.Instance.ConfirmBeforeRun;
            _ChangeExePathCmd = new RelayCommand(ExecChangeExePath, (o) => { return true; });
            _ChangeSavedCommandsLocCmd = new RelayCommand(ExecChangeSavedCommandsLoc, (o) => { return true; });
            _ResetSettingsCmd = new RelayCommand(ExecResetSettingsCmd, CanResetSettingsCmd);
        }

        private void ExecResetSettingsCmd(object obj)
        {
            //Perfom ResetSettingsCmd Activity
            Settings.Instance.Reset();
            Settings.Instance.Save();
            SavedCommandsPath = Settings.Instance.SavedCommandsPath;
            ExePath = Settings.Instance.ExePath;
            StatusMessage = "Settings were reset to default!";
            ObjMessage message = new ObjMessage("MainView","ResetSettings");
            EventAggregator.Publish(message);
        }

        private bool CanResetSettingsCmd(object obj)
        {
            //Add Enable-disable logic here
            return (true);
        }

        private void ExecChangeSavedCommandsLoc(object obj)
        {
            // Configure open file dialog box
            var dlg = new System.Windows.Forms.FolderBrowserDialog();

            // Show open file dialog box
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Open document 
                SavedCommandsPath = dlg.SelectedPath;
                Settings.Instance.SavedCommandsPath = SavedCommandsPath;
                Settings.Instance.Save();
                StatusMessage = "Saved Commands Path Updated!";
                ObjMessage message = new ObjMessage("MainView", "UpdateSavedCommandsPath");
                EventAggregator.Publish(message);
            }
        }

        private void ExecChangeExePath(object obj)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Executable Files (.exe)|*.exe|Batch Files (.bat)|*.bat"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                ExePath = dlg.FileName;
                Settings.Instance.ExePath = ExePath;
                Settings.Instance.Save();
                StatusMessage = "Exe Path Updated!";
                ObjMessage message = new ObjMessage("MainView","UpdateExePath");
                EventAggregator.Publish(message);
            }
        }

        private string _StatusMessage;

        public string StatusMessage
        {
            get { return _StatusMessage; }
            set
            {
                if (_StatusMessage != value)
                {
                    _StatusMessage = value;
                    OnPropertyChanged("StatusMessage");
                }
            }
        }

        public string SavedCommandsPath
        {
            get { return _SavedCommandsPath; }
            set { _SavedCommandsPath = value;
            OnPropertyChanged("SavedCommandsPath");
            }
        }

        public string ExePath
	    {
		    get { return _ExePath;}
		    set { _ExePath = value;
            OnPropertyChanged("ExePath");
            }
	    }


        public bool SaveLogFiles
        {
            get { return _SaveLogFiles; }
            set { 
                _SaveLogFiles = value;
                Settings.Instance.IsSaveLogsEnabled = value;
                Settings.Instance.Save();
                OnPropertyChanged("SaveLogFiles");
                StatusMessage = string.Format("Log files will {0} be saved!",value?"":"not");
                ObjMessage message = new ObjMessage("MainView","SaveLogFiles");
                EventAggregator.Publish(message);
            }
        }

        public bool ConfirmBeforeRun
        {
            get { return _ConfirmBeforeRun; }
            set
            {
                _ConfirmBeforeRun = value;
                Settings.Instance.ConfirmBeforeRun = value;
                Settings.Instance.Save();
                OnPropertyChanged("ConfirmBeforeRun");
                StatusMessage = string.Format("Will {0} confirm before running batch files!", value ? "" : "not");
                ObjMessage message = new ObjMessage("MainView","ConfirmBeforeRun");
                EventAggregator.Publish(message);
            }
        }

    }
}
