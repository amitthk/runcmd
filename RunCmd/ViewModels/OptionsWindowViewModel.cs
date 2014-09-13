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
        
        private readonly IMessageBus _messageBus;

        public IMessageBus MessageBus
        {
            get { return _messageBus; }
        }

        public OptionsWindowViewModel()
        {
            //Dirty instantiate IMessageBus = shoud use Dependency Injection in Real App
            _messageBus = App.messageBus;
            _ExePath = Settings.Instance.ExePath;
            _SavedCommandsPath = Settings.Instance.SavedCommandsPath;
            _ChangeExePathCmd = new RelayCommand(ExecChangeExePath, (o) => { return true; });
            _ChangeSavedCommandsLocCmd = new RelayCommand(ExecChangeSavedCommandsLoc, (o) => { return true; });
            _ResetSettingsCmd = new RelayCommand(ExecResetSettingsCmd, CanResetSettingsCmd);
        }

        private void ExecResetSettingsCmd(object obj)
        {
            //Perfom ResetSettingsCmd Activity
            Settings.Instance.Reset();
            SavedCommandsPath = Settings.Instance.SavedCommandsPath;
            ExePath = Settings.Instance.ExePath;
            NavMessage message = new NavMessage("ResetSettings");
            MessageBus.Publish(message);
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
                NavMessage message = new NavMessage("UpdateSavedCommandsPath");
                MessageBus.Publish(message);
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
                NavMessage message = new NavMessage("UpdateExePath");
                MessageBus.Publish(message);
            }
        }
	    
        private string _ExePath;
        private string _SavedCommandsPath;

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
    }
}
