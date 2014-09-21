using RunCmd.Common;
using RunCmd.Common.Messaging;
using System;
using System.Collections.Generic;
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
        private IMessageBus _messageBus;


        public ICommand OpenOptionsWinCmd { get { return (_OpenOptionsWinCmd); } }
        public ICommand ExitCmd { get { return (_exitCmd); } }
        public ICommand OpenLogWinCmd { get { return (_OpenLogWinCmd); } }


        public MainWindowViewModel()
        {
            _messageBus = App.messageBus;
            _exitCmd = new RelayCommand(ExecExit, (obj) => true);
            _OpenOptionsWinCmd = new RelayCommand(ExecOpenOptionsWinCmd, CanOpenOptionsWin);
            _OpenLogWinCmd = new RelayCommand(ExecOpenLogWinCmd, CanOpenLogWinCmd);

        }

        private bool CanOpenLogWinCmd(object obj)
        {
            return (true);
        }

        private void ExecOpenLogWinCmd(object obj)
        {
            _messageBus.Publish<NavigationMessage>(new NavigationMessage("LogsView"));
        }

        private void ExecExit(object obj)
        {
            Application.Current.Shutdown();
        }

        private bool CanOpenOptionsWin(object obj)
        {
            return (true);
        }

        private void ExecOpenOptionsWinCmd(object obj)
        {
            var optWin = new OptionsWindow();
            optWin.ShowDialog();
        }
    }
}
