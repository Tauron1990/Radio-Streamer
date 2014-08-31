using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Tauron;
using Tauron.Application.BassLib;
using Tauron.Application.BassLib.Channels;
using Tauron.Application.BassLib.Encoder;
using Tauron.Application.BassLib.Misc;
using Un4seen.Bass;
using Un4seen.Bass.Misc;

namespace TestOnline
{
    public partial class MainForm : Form
    {
        private const string Password = "testServer";
        private const string AdminPassword = "testServer1";
        private const string ServerAdress = "127.0.0.1";

        private MemoryManager _memoryManager;

        private readonly List<string> _files = new List<string>();
        private readonly List<string> _allfiles = new List<string>();

        private readonly Random _random =
            new Random(unchecked(DateTime.Now.Year + DateTime.Now.DayOfYear + DateTime.Now.Second));

        private AudioBroadCast _broadCast;
        private Mix _mix;
        private Channel _channel;
        private BassEngine _bassEngine;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            BassManager.Free();
            _memoryManager.Dispose();
        }

        private void backgroundLoader_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Write(@"Initialize Core System...  ");


            BassNet.Registration("Game-over-Alexander@web.de", "2X1533726322323");
            Thread.Sleep(500);
            BassManager.InitBass();

            _memoryManager = new MemoryManager();
            _memoryManager.Init();

            _bassEngine = new BassEngine();

            _mix = new Mix(flags: BassMixFlags.Nonstop);
            var lame = new LameEncoder(_mix)
            {
                InputFile = null,
                OutputFile = null,
                Bitrate = BaseEncoder.BITRATE.kbps_256
            };

            var castServer = new ShoutCastServer(lame)
            {
                ServerAddress = ServerAdress,
                Port = 8001,
                Password = Password,
                AdminPassword = AdminPassword,
                PublicFlag = true
            };

            _broadCast = new AudioBroadCast(castServer) { AutoReconnect = true };
            _broadCast.Notification += BroadCastOnNotification;

            WriteLine(@"Done");
            WriteLine("Running on Adress: " + ServerAdress);
            WriteLine();
            WriteLine(@"Waiting for Startup");
        }

        public bool SuspendClearing { get; set; }

        private void WriteLine(string line = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(WriteLine), line);
                return;
            }

            if(ConsoleContent.Text.Length > 2500 && !SuspendClearing)
                Clear();

            ConsoleContent.Text += line + Environment.NewLine;
        }

        private void Write(string line)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(Write), line);
                return;
            }

            ConsoleContent.Text += line;
        }

        private void WriteError(object error, bool disableAll = true)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, bool>(WriteError), error, disableAll);
                return;
            }

            SuspendClearing = true;

            WriteLine();
            WriteLine("Error:");
            WriteLine(error.ToString());
            
            Disable(stop);
            Disable(start);
            BassManager.Free();

            WriteLine();
            WriteLine("Restart App!");
        }

        private void Clear()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Clear));
                return;
            }

            ConsoleContent.Text = string.Empty;
        }

        private void Enable(Control control)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Control>(Enable), control);
                return;
            }

            control.Enabled = true;
        }

        private void Disable(Control control)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Control>(Disable), control);
                return;
            }

            control.Enabled = false;
        }

        private void BroadCastOnNotification(object sender, BroadCastEventArgs broadCastEventArgs)
        {
            switch (broadCastEventArgs.EventType)
            {
                case BroadCastEventType.EncoderStartError:
                case BroadCastEventType.EncoderStopError:
                case BroadCastEventType.TitleUpdateError:
                    WriteError("Bass Error: " + _bassEngine.GetLastError());
                    break;
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            Disable(stop);
            Enable(start);
            Write("Stopping Server... ");
            _broadCast.Disconnect();
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
            WriteLine("Done");
        }

        private void backgroundLoader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                WriteError(e);
                return;
            }

            Enable(start);
            WriteLine();
        }

        private void start_Click(object sender, EventArgs e)
        {
            Disable(start);
            startStreamingWorker.RunWorkerAsync();
        }

        private void startStreamingWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Write("Starting Server... ");

            _allfiles.Clear();
            _allfiles.AddRange("Music".GetFiles("*.mp3", SearchOption.TopDirectoryOnly));
            FillFileQueue();
            _broadCast.AutoConnect();

            Enable(stop);

            WriteLine("Done");
            WriteLine(_allfiles.Count + " Files");
            WriteLine();

            PlayNextFile();
        }

        private void FillFileQueue()
        {
            foreach (var file in _allfiles)
            {
                _files.Add(file);
            }
        }

        private void PlayNextFile()
        {
            Disable(stop);
            Write("Next File... ");

            if(_files.Count == 0)
                FillFileQueue();

            int next = _random.Next(0, _files.Count - 1);
            string file = _files[next];
            _files.RemoveAt(next);

            if (_channel != null)
            {
                _mix.DeAttach(_channel);
                _channel.Dispose();
            }

            var title = _bassEngine.CreateFile(file, flags: FileFlags.Decode);
            title.SetEntSync(playNextFileWorker.RunWorkerAsync);
            _mix.Attach(title);
            _channel = title;

            _broadCast.UpdateTitle(file.GetFileNameWithoutExtension(), "Zero");

            WriteLine(file.GetFileName());
            Enable(stop);
        }

        private void playNextFileWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            PlayNextFile();
        }

        private void RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) WriteError(e.Error);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            backgroundLoader.RunWorkerAsync();
        }
    }
}
