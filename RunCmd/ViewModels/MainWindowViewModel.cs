using RunCmd.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;
using RunCmd.Common.Messaging;
using RunCmd.Common.Helpers;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Collections;

namespace RunCmd.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ICommand _runCmd;
        private readonly ICommand _saveCmd;
        private readonly ICommand _exitCmd;
        private readonly ICommand _stopCmd;
        private readonly ICommand _OpenOptionsWinCmd;

        private ICommand _selectBatFileCmd;

        public ICommand RunCmd { get { return (_runCmd); } }
        public ICommand SaveCmd { get { return (_saveCmd); } }
        public ICommand ExitCmd { get { return (_exitCmd); } }
        public ICommand StopCmd { get { return (_stopCmd); } }
        public ICommand ExeFilePickCmd { get { return (_exeFilePickCmd); } }
        public ICommand OpenOptionsWinCmd { get { return (_OpenOptionsWinCmd); } }
        public ICommand SelectBatFileCmd { get { return (_selectBatFileCmd); } }

        private string _cmdText;
        private Thread _workerThread;
        public string TxtOutput { get { return (_txtOutput); } set { _txtOutput = value; OnPropertyChanged("TxtOutput"); } }
        public bool _isCmdRunning { get; set; }
        StringBuilder buffer = new StringBuilder();
        private static int numOutputLines;

        private string _txtOutput;
        private ICommand _exeFilePickCmd;
        private Process _process;
        private bool _IsCustomExe;
        private string _ExeFileName;
        private string _SavedCommandsLoc;
        private ObservableCollection<string> _batFileNames;
        private string _selectedBatFile;

        public string SelectedBatFile
        {
            get { return _selectedBatFile; }
            set
            {
                if ((_selectedBatFile != value)&&(value!=null))
                {
                    _selectedBatFile = value;
                    CmdText = Utility.ReadFileString(_selectedBatFile);
                    OnPropertyChanged("SelectedBatFile");
                }
            }
        }


        ManualResetEvent _threadResetHandle = new ManualResetEvent(false);

        public bool IsCustomExe
        {
            get { return _IsCustomExe; }
            set {
                if (_IsCustomExe != value)
                {
                    _IsCustomExe = value;
                    if (!value) { ExeFileName = "cmd.exe"; }
                    else { ExeFileName = Settings.Instance.ExePath; }
                    OnPropertyChanged("IsCustomExe");
                }
                }
        }

        public bool IsCmdRunning
        {
            get { return _isCmdRunning; }
            set {
                if (_isCmdRunning != value)
                {
                    _isCmdRunning = value;
                    OnPropertyChanged("IsCmdRunning");
                }
            
            
            }
        }


        public ObservableCollection<string> BatFileNames
        {
            get { return _batFileNames; }
            set {
                if (_batFileNames != value)
                {
                    _batFileNames = value;
                    OnPropertyChanged("BatFileNames");
                }
            }
        }


        private readonly IMessageBus _messageBus;

        public IMessageBus MessageBus
        {
            get { return _messageBus; }
        }



        public string CmdText
        {
            get { return _cmdText; }
            set { 
                _cmdText = value;
                OnPropertyChanged("CmdText");
            }
        }

        public string ExeFileName
        {
            get { return _ExeFileName; }
            set { _ExeFileName = value;
            OnPropertyChanged("ExeFileName");
            }
        }



        public string SavedCommandsLoc
        {
            get { return _SavedCommandsLoc; }
            set { _SavedCommandsLoc = value;
            BatFileNames = new ObservableCollection<string>(Utility.GetAllFileNames(_SavedCommandsLoc, "*.bat"));
            OnPropertyChanged("SavedCommandsLoc");
            }
        }



        public MainWindowViewModel()
        {
            //Dirty instantiate IMessageBus = shoud use Dependency Injection in Real App
            _messageBus = App.messageBus;

            _runCmd = new RelayCommand(ExecRunCmd, CanRunCmd);
            _saveCmd = new RelayCommand(ExecSaveCmd, CanSaveCmd);
            _stopCmd = new RelayCommand(ExecStopCmd, CanStopCmd);
            _exitCmd= new RelayCommand(ExecExit, (obj) => true);
            _exeFilePickCmd = new RelayCommand(ExecExeFilePickCmd, CanChangeExeFile);
            _OpenOptionsWinCmd = new RelayCommand(ExecOpenOptionsWinCmd, CanOpenOptionsWin);
            _selectBatFileCmd = new RelayCommand(ExecSelectedBatFile, CanSelectedBatFile);
            ExeFileName = "cmd.exe"; //Settings.Instance.ExePath;
            
            string cmdPath= string.IsNullOrWhiteSpace(Settings.Instance.SavedCommandsPath)?Utility.SavedCommandsDefaultPath:Settings.Instance.SavedCommandsPath;
            _batFileNames=new ObservableCollection<string>(Utility.GetAllFileNames(cmdPath,"*.bat"));
            MessageBus.Subscribe<NavMessage>(HandleOptionsUpdated);
            _isCmdRunning = false;
        }

        private bool CanSelectedBatFile(object obj)
        {
            return (!IsCmdRunning);
        }

        private void ExecSelectedBatFile(object obj)
        {
            CmdText = Utility.ReadFileString(SelectedBatFile);
        }

        private bool CanStopCmd(object obj)
        {
            return (IsCmdRunning);
        }

        private void ExecStopCmd(object obj)
        {
            _threadResetHandle.Set();
            if (!_process.HasExited)
            {
                _process.Kill();
            }
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

        private bool CanChangeExeFile(object obj)
        {
            return(IsCustomExe);
        }

        private void ExecExeFilePickCmd(object obj)
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
                ExeFileName = dlg.FileName;
                Settings.Instance.ExePath = ExeFileName;
                Settings.Instance.Save();
            }
        }

        private void HandleOptionsUpdated(NavMessage obj)
        {
            switch (obj.EventType)
            {
                case ("UpdateSavedCommandsPath"):
                    SavedCommandsLoc = Settings.Instance.SavedCommandsPath;                    
                    break;
                case ("UpdateExePath"):
                    ExeFileName = Settings.Instance.ExePath;
                    break;
                case("ResetSettings"):
                    ExeFileName = Settings.Instance.ExePath;
                    SavedCommandsLoc = Settings.Instance.SavedCommandsPath;
                    break;
                default:
                    break;
            }
        }

        private void ExecRunCmd(object obj)
        {
            try
            {
                //string[] cmdArgs = new string[]
                //    {
                //        "/C ",
                //        CmdText
                //    };



                

                string strCmdArgs = " ";

                if (ExeFileName.Equals("cmd.exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    strCmdArgs += " /C ";
                }

                strCmdArgs += CmdText;

                //if (!string.IsNullOrWhiteSpace(CmdText))
                //{
                //    List<string> cmdArgs = new List<string>(CmdText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                //    var hs= GetCommandLineArgs(cmdArgs.ToArray());

                //    foreach (string key in hs.Keys)
                //    {
                //        strCmdArgs += key + " " + hs[key];
                //    }
                //}



                _process = new Process();
                _process.StartInfo = new ProcessStartInfo(ExeFileName)
                {
                    Arguments = strCmdArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Utility.getAppBasePath()
                };

                _process.OutputDataReceived += new DataReceivedEventHandler(SortOutputHandler);
                _process.OutputDataReceived += new DataReceivedEventHandler((o, d) => AppendText(d.Data));
                _process.ErrorDataReceived += new DataReceivedEventHandler((o, d) => AppendError(d.Data));
                _process.EnableRaisingEvents = true;
                _process.Exited += new EventHandler(BuildCompleted);

                TxtOutput = string.Empty; //Initialize the output first

                    ThreadStart ths = new ThreadStart(() =>
                    {
                        bool ret = _process.Start();
                        _process.BeginOutputReadLine();
                        //is ret what you expect it to be....
                    });
                    _workerThread = new Thread(ths);
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                    IsCmdRunning = true;


                _workerThread.Join();
                IsCmdRunning = false;

            }
            catch (Exception exc)
            {
                throw (exc);
            }
        }

        private Hashtable GetCommandLineArgs(string[] args) {
            Hashtable CommandLineArgs = new Hashtable();
            // Don't bother if no command line args were passed
            // NOTE: e.Args is never null - if no command line args were passed, 
            //       the length of e.Args is 0.
            if (args.Length==0) { return (null); }

            // Parse command line args for args in the following format:
            //   /argname:argvalue /argname:argvalue /argname:argvalue ...
            //
            // Note: This sample uses regular expressions to parse the command line arguments.
            // For regular expressions, see:
            // http://msdn.microsoft.com/library/en-us/cpgenref/html/cpconRegularExpressionsLanguageElements.asp
            string pattern = @"(?<argname>/\w+):(?<argvalue>\w+)";
            foreach (string arg in args)
            {
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(arg, pattern);

                // If match not found, command line args are improperly formed.
                if (!match.Success) throw new ArgumentException("The command line arguments are improperly formed. Use /argname:argvalue.");

                // Store command line arg and value
                CommandLineArgs[match.Groups["argname"].Value] = match.Groups["argvalue"].Value;
            }
            return (CommandLineArgs);
        }

        private void AppendError(string text)
        {
            if (text != null)
            {
                TxtOutput += "[ERROR]"+text+"[ERROR]";
                TxtOutput += Environment.NewLine;
            }
        }

        private void BuildCompleted(object sender, EventArgs args)
        {
            Process proc = (Process)sender;
            IsCmdRunning = false;
            if (proc.ExitCode!=0)
            {
                string erTxt = string.Format("[ERROR] Process exited with error! Exit Code: {0} [ERROR]", proc.ExitCode);
                buffer.Append(erTxt);
                AppendError(erTxt);
            }
            Utility.WriteToFile (buffer.ToString(),Utility.getAsolutePathForRelativeFileName("log",Utility.DateTimeStampAsString+".log"));
            _threadResetHandle.Set();
            proc.Dispose();
        }

        private void SortOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                numOutputLines++;
                string txt = Environment.NewLine +
                    "[" + numOutputLines.ToString() + "] - " + outLine.Data;
                // Add the text to the collected output.
                buffer.Append(txt);
            }
        }

        private void ExecSaveCmd(object obj)
        {
            string batFileStoreLoc= Utility.SavedCommandsDefaultPath;
            if(Utility.DirExists(Settings.Instance.SavedCommandsPath)){
            batFileStoreLoc=Settings.Instance.SavedCommandsPath;
            }
            Utility.WriteToFile(CmdText, batFileStoreLoc+"\\"+Utility.DateTimeStampAsString+".bat");
            BatFileNames = new ObservableCollection<string>(Utility.GetAllFileNames(batFileStoreLoc, "*.bat"));
        }

        private void ExecExit(object obj)
        {
            Application.Current.Shutdown();
        }

        private bool CanSaveCmd(object obj)
        {
            //Enable the Button only if the mandatory fields are filled
            if (CmdText != string.Empty)
                return true;
            return false;
        }

        private bool CanRunCmd(object obj)
        {
            //Enable the Button only if the mandatory fields are filled
            if ((!string.IsNullOrWhiteSpace(CmdText))&&(!IsCmdRunning))
                return true;
            return false;
        }



        private void AppendText(string text)
        {
            if (text != null)
            {
                TxtOutput += text;
                TxtOutput += Environment.NewLine;
            }
        }

    }
}
