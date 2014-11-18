using RunCmd.Common;
using RunCmd.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RunCmd.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private readonly ICommand _OpenOptionsWinCmd;
        private readonly ICommand _exitCmd;
        private readonly ICommand _OpenLogWinCmd;
        private readonly ICommand _MinimizeToTrayCmd;
        private IEventAggregator _eventAggregator;

        public ICommand OpenOptionsWinCmd { get { return (_OpenOptionsWinCmd); } }
        public ICommand ExitCmd { get { return (_exitCmd); } }
        public ICommand OpenLogWinCmd { get { return (_OpenLogWinCmd); } }
        public ICommand MinimizeToTrayCmd { get { return(_MinimizeToTrayCmd);} }



        public MainWindowViewModel()
        {
            _eventAggregator = App.eventAggregator;
            _exitCmd = new RelayCommand(ExecExit, (obj) => true);
            _OpenOptionsWinCmd = new RelayCommand(ExecOpenOptionsWinCmd, CanOpenOptionsWin);
            _OpenLogWinCmd = new RelayCommand(ExecOpenLogWinCmd, CanOpenLogWinCmd);
            _MinimizeToTrayCmd = new RelayCommand(ExecMinimizeToTrayCmd, CanMinimizeToTray);
        }

        //These methods are checked again & again in loop, so we will step through them while debugging
        [DebuggerStepThrough]
        private bool CanMinimizeToTray(object obj)
        {
            return (true);
        }

        private void ExecMinimizeToTrayCmd(object obj)
        {
            var win= (Window)obj;

            var notifyIcon= RunCmdNotifyIcon.Instance.GetIcon(win);

            notifyIcon.Visible = true;
            win.Hide();
        }


        [DebuggerStepThrough]
        private bool CanOpenLogWinCmd(object obj)
        {
            return (true);
        }

        private void ExecOpenLogWinCmd(object obj)
        {
            _eventAggregator.Publish<NavMessage>(new NavMessage("LogsView"));
        }

        private void ExecExit(object obj)
        {
            Application.Current.Shutdown();
        }

        [DebuggerStepThrough]
        private bool CanOpenOptionsWin(object obj)
        {
            return (true);
        }

        [DebuggerStepThrough]
        private void ExecOpenOptionsWinCmd(object obj)
        {
            var optWin = new OptionsWindow();
            optWin.ShowDialog();
        }


    }
}
