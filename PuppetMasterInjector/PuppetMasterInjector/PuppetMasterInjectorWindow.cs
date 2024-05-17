using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using HoLLy.ManagedInjector;
using PuppetMaster;
namespace PuppetMasterInjector
{
    public partial class PuppetMasterInjectorWindow : Form
    {
        private Thread listenerThread;
        private CancellationTokenSource source;
        public PuppetMasterInjectorWindow()
        {
            InitializeComponent();
            listenerThread = new Thread(() => StartListener(source.Token));
            source = new CancellationTokenSource();
            listenerThread.Start();
        }

        private bool VerifyDirectory()
        {
            return File.Exists($"{gameDirPath.Text}\\TowerFall.exe");
        }
        private void InjectBtn_Click(object sender, EventArgs e)
        {
            this.InjectBtn.Enabled = false;

            LogToBox("Starting loader\n");

            if (!VerifyDirectory())
            {
                LogToBox("Directory seems invalid.\n", Color.Red);
                return;
            }

            CopyDependencies();

            Process p = Launch();

            p.WaitForInputIdle();

            LogToBox("Waiting...\n");

            Thread.Sleep(1000);

            Inject(p.Id);

            this.InjectBtn.Enabled = true;

        }

        private void StartListener(CancellationToken token)
        {
           
            LogToBox("Listening for connection...\n");
            try
            {
                NamedPipeServerStream server = new NamedPipeServerStream("tfmods", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                //server.WaitForConnection();
                
                server.BeginWaitForConnection(new AsyncCallback(ConnectionMade), new object [] { server, token });
            } catch (Exception e)
            {
                LogToBox("Failed to start listener\n", Color.Red);
            }
        }

        private void ConnectionMade(IAsyncResult ar)
        {
            LogToBox("Connection made\n");
            object[] states = (object[]) ar.AsyncState;
            NamedPipeServerStream server = (NamedPipeServerStream)states[0];
            CancellationToken token = (CancellationToken)states[1];
            StreamReader reader = new StreamReader(server);
            LogToBox("Connected\n");
            while (!token.IsCancellationRequested)
            {
                var line = reader.ReadLine();
                if (line != null)
                    LogToBox($"{line}\n", Color.Blue);
            }
            reader.Close();
            server.Close();
        }

        private void CopyDependencies()
        {
            LogToBox("Copying dependencies\n");
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + "\\PuppetMasterTools.dll", $"{gameDirPath.Text}\\PuppetMasterTools.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\Harmony\\0Harmony.dll", $"{gameDirPath.Text}\\0Harmony.dll", true);
            }
            catch
            {
                LogToBox("Something went wrong...\n", Color.Red);
                return;
            };
            LogToBox("Copied dependencies\n");
        }

        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
        private void LogToBox(string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(LogToBox), new object[] { text });
                return;
            }
            AppendText(richTextBox1, text, Color.Black);
        }

        private void LogToBox(string text, Color color)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(AppendText, new object[] { richTextBox1, text, color });
                return;
            }
            AppendText(richTextBox1, text, color);
        }
        private Process Launch()
        {
            var startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = gameDirPath.Text;
            startInfo.FileName = $"{gameDirPath.Text}\\TowerFall.exe";
            
            Process p = Process.Start(startInfo);

            LogToBox("Process started\n");
            return p;
        }

        private void Inject(int pid)
        {
            LogToBox("Starting injection\n");

            try
            {
                var process = new InjectableProcess(Convert.ToUInt32(pid));

                Assembly ass = Assembly.LoadFile(Assembly.GetAssembly(typeof(Modloader)).Location);

                process.Inject($"{Directory.GetCurrentDirectory()}\\modloader.dll", "PuppetMaster.Modloader", "Inject");

                LogToBox("Injected!\n");
            }
            catch (Exception e)
            {
                LogToBox("Falling back on steam injection...\n", Color.Red);
                Thread.Sleep(1000);
                LogToBox("Finding Process...\n");
                Process[] Ps = Process.GetProcessesByName("TowerFall");
                if (Ps.Length > 0)
                {
                    Process p = Ps[0];

                    var process = new InjectableProcess(Convert.ToUInt32(p.Id));

                    Assembly ass = Assembly.LoadFile(Assembly.GetAssembly(typeof(Modloader)).Location);

                    process.Inject($"{Directory.GetCurrentDirectory()}\\modloader.dll", "PuppetMaster.Modloader", "Inject");

                    LogToBox("Injected!\n");
                }
            }

            
        }

        private void LogToFile(String Text)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:/Users/jake_", "Injector.txt"), true))
            {
                outputFile.WriteLine(Text);
            }
        }

        private void PuppetMasterInjectorWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            source.Cancel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = gameDirChooser.ShowDialog();

            if(result == DialogResult.OK)
            {
                gameDirPath.Text = gameDirChooser.SelectedPath;
            }
            InjectBtn.Enabled = true;
        }
    }
}