using RunCmd.Common;
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


        public ICommand OpenOptionsWinCmd { get { return (_OpenOptionsWinCmd); } }
        public ICommand ExitCmd { get { return (_exitCmd); } }


        public MainWindowViewModel()
        {
            _exitCmd = new RelayCommand(ExecExit, (obj) => true);
            _OpenOptionsWinCmd = new RelayCommand(ExecOpenOptionsWinCmd, CanOpenOptionsWin);

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
