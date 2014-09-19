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
using System.IO;

namespace RunCmd.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ICommand _runCmd;
        private readonly ICommand _saveCmd;
        private readonly ICommand _NewBatFileCmd;
        private readonly ICommand _exitCmd;
        private readonly ICommand _stopCmd;
        private readonly ICommand _OpenOptionsWinCmd;
        private readonly ICommand _SendInputCmd;

        public ICommand RunCmd { get { return (_runCmd); } }
        public ICommand SaveCmd { get { return (_saveCmd); } }
        public ICommand NewBatFileCmd { get { return (_NewBatFileCmd); } }
        public ICommand ExitCmd { get { return (_exitCmd); } }
        public ICommand StopCmd { get { return (_stopCmd); } }
        public ICommand ExeFilePickCmd { get { return (_exeFilePickCmd); } }
        public ICommand OpenOptionsWinCmd { get { return (_OpenOptionsWinCmd); } }
        public ICommand SendInputCmd { get { return (_SendInputCmd); } }

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
        private ObservableCollection<BatFileViewModel> _BatFiles;
        private BatFileViewModel _selectedBatFile;
        private string _TxtInput;

        public string TxtInput
        {
            get { return _TxtInput; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _TxtInput = value;
                    OnPropertyChanged(() => TxtInput);
                }
            }
        }

        

        public BatFileViewModel SelectedBatFile
        {
            get { return _selectedBatFile; }
            set
            {
                if ((_selectedBatFile != value)&&(value!=null))
                {
                    _selectedBatFile = value;
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


        public ObservableCollection<BatFileViewModel> BatFiles
        {
            get { return _BatFiles; }
            set {
                if (_BatFiles != value)
                {
                    _BatFiles = value;
                    OnPropertyChanged("BatFiles");
                }
            }
        }


        private readonly IMessageBus _messageBus;

        public IMessageBus MessageBus
        {
            get { return _messageBus; }
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
            set {
                if (_SavedCommandsLoc != value)
                {
                    _SavedCommandsLoc = value;
                    BatFiles = LoadBatFiles();
                    OnPropertyChanged("SavedCommandsLoc");
                }
            }
        }

        private ObservableCollection<BatFileViewModel> LoadBatFiles()
        {
            ObservableCollection<BatFileViewModel> rtrn= null;
            if (!Utility.DirExists(SavedCommandsLoc))
            {
                SavedCommandsLoc = Utility.SavedCommandsDefaultPath;
            }
            var lstitms = Utility.LoadAllBatFiles(SavedCommandsLoc, "*.bat");

            if ((lstitms!=null)&&(lstitms.Count>0))
            {
                rtrn = new ObservableCollection<BatFileViewModel>();
                foreach (var item in lstitms)
                {
                    rtrn.Add(item);
                }
            }
            return (rtrn);
        }



        public MainWindowViewModel()
        {
            //Dirty instantiate IMessageBus = shoud use Dependency Injection in Real App
            _messageBus = App.messageBus;

            _runCmd = new RelayCommand(ExecRunCmd, CanRunCmd);
            _saveCmd = new RelayCommand(ExecSaveCmd, CanSaveCmd);
            _NewBatFileCmd = new RelayCommand(ExecNewBatFileCme, CanExecNewBatFileCmd);
            _stopCmd = new RelayCommand(ExecStopCmd, CanStopCmd);
            _exitCmd= new RelayCommand(ExecExit, (obj) => true);
            _exeFilePickCmd = new RelayCommand(ExecExeFilePickCmd, CanChangeExeFile);
            _OpenOptionsWinCmd = new RelayCommand(ExecOpenOptionsWinCmd, CanOpenOptionsWin);
            _SendInputCmd = new RelayCommand(ExecSendInputCmd, CanSendInputCmd);
            ExeFileName = "cmd.exe"; //Settings.Instance.ExePath;
            
            string cmdPath= string.IsNullOrWhiteSpace(Settings.Instance.SavedCommandsPath)?Utility.SavedCommandsDefaultPath:Settings.Instance.SavedCommandsPath;
            SavedCommandsLoc = cmdPath;
            _BatFiles = LoadBatFiles();//new ObservableCollection<BatFileViewModel>(Utility.LoadAllBatFiles(cmdPath,"*.bat"));
            _selectedBatFile = new BatFileViewModel();
            MessageBus.Subscribe<NavMessage>(HandleOptionsUpdated);
            _isCmdRunning = false;
        }

        private bool CanSendInputCmd(object obj)
        {
            return (IsCmdRunning);
        }

        private void ExecSendInputCmd(object obj)
        {
            _process.StandardInput.WriteLine(TxtInput);
        }

        private void ExecNewBatFileCme(object obj)
        {
            SelectedBatFile = new BatFileViewModel();
        }

        private bool CanExecNewBatFileCmd(object obj)
        {
            return(!IsCmdRunning);
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
            SaveOrUpdateCurrentBatFile();
            try
            {
                string[] cmdArgs = new string[]
                    {
                        "cmd /C ",
                        "\""+SelectedBatFile.BatFileName+"\""
                    };



                

                //string strCmdArgs = " ";

                //if (ExeFileName.Equals("cmd.exe", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    strCmdArgs += " /C ";
                //}

                //strCmdArgs += SelectedBatFile.CmdText;

                //if (!string.IsNullOrWhiteSpace(SelectedBatFile.CmdText))
                //{
                //    //Clean up empty new lines and insert newLines back at end of every non-empty line
                //    List<string> cmdArgs = new List<string>(SelectedBatFile.CmdText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                //    foreach (string cmdarg in cmdArgs)
                //    {
                //        strCmdArgs += cmdarg + "\r\n";
                //    }
                //}



                _process = new Process();
                _process.StartInfo = new ProcessStartInfo(ExeFileName)
                {
                    Arguments = string.Join(" ",cmdArgs),
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

            }
            catch (Exception exc)
            {
                throw (exc);
            }
        }

        private void SaveOrUpdateCurrentBatFile()
        {
            string batFileToSave = SelectedBatFile.BatFileName;

            if (!SelectedBatFile.BatFileName.EndsWith(".bat"))
	        {
                SelectedBatFile.BatFileNameDisplay+=".bat";
	        }

            if (SavedCommandsLoc.Equals(Utility.SavedCommandsDefaultPath))
            {
                batFileToSave = SelectedBatFile.BatFileName;
            }
            else //If it is not the default location, don't override the original batch file, rather create new one
            {
                while (Utility.Exists(SelectedBatFile.BatFileName))
                {
                    SelectedBatFile.BatFileName = SelectedBatFile.BatFileName.Substring(0, SelectedBatFile.BatFileName.Length - 4) + "_1.bat";
                }
                batFileToSave = SelectedBatFile.BatFileName;
            }

            Utility.WriteToFile(SelectedBatFile.CmdText, batFileToSave);
            BatFiles = LoadBatFiles();
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
            SaveOrUpdateCurrentBatFile();
        }

        private void ExecExit(object obj)
        {
            Application.Current.Shutdown();
        }

        private bool CanSaveCmd(object obj)
        {
            //Enable the Button only if the mandatory fields are filled
            return (!string.IsNullOrWhiteSpace((SelectedBatFile.CmdText)));
        }

        private bool CanRunCmd(object obj)
        {
            //Enable the Button only if the mandatory fields are filled
            if ((!string.IsNullOrWhiteSpace(SelectedBatFile.CmdText))&&(!IsCmdRunning))
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
