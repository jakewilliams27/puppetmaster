using System.Diagnostics;
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
        public PuppetMasterInjectorWindow()
        {
            InitializeComponent();
            new Thread(StartListener).Start();
        }

        private void Inject_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;

            LogToBox("Starting loader\n");

            CopyDependencies();
            

            Process p = Launch();

            p.WaitForInputIdle();

            LogToBox("Waiting...\n");

            Thread.Sleep(1000);

            Inject(p.Id);

            this.button1.Enabled = true;

        }

        private void StartListener()
        {
           
                LogToBox("Listening for connection...\n");
                var server = new NamedPipeServerStream("tfmods");
                server.WaitForConnection();
                StreamReader reader = new StreamReader(server);
                StreamWriter writer = new StreamWriter(server);
                LogToBox("Connected\n");
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                        LogToBox($"{line}\n");
                }
        }
        private void CopyDependencies()
        {
            LogToBox("Copying dependencies\n");
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + "\\PuppetMasterTools.dll", "C:\\GOG Games\\Towerfall - Ascension\\PuppetMasterTools.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\Harmony\\0Harmony.dll", "C:\\GOG Games\\Towerfall - Ascension\\0Harmony.dll", true);
            }
            catch
            {
                LogToBox("Something went wrong...\n");
                return;
            };
            LogToBox("Copied dependencies\n");
        }

        private void LogToBox(string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(LogToBox), new object[] { text });
                LogToFile(text);
                return;
            }
            this.richTextBox1.Text += text;
        }
        private Process Launch()
        {
            var startInfo = new ProcessStartInfo();

            startInfo.WorkingDirectory = "C:\\GOG Games\\Towerfall - Ascension\\";
            startInfo.FileName = "C:\\GOG Games\\Towerfall - Ascension\\TowerFall.exe";
            
            Process p = Process.Start(startInfo);

            LogToBox("Process started\n");
            return p;
        }

        private void Inject(int pid)
        {
            LogToBox("Starting injection\n");

            var process = new InjectableProcess(Convert.ToUInt32(pid));

            Assembly ass = Assembly.LoadFile(Assembly.GetAssembly(typeof(Modloader)).Location);
            
            process.Inject($"{Directory.GetCurrentDirectory()}\\modloader.dll", "PuppetMaster.Modloader", "Inject");

            LogToBox("Injected!\n");
        }

        private void LogToFile(String Text)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:/Users/jake_", "Injector.txt"), true))
            {
                outputFile.WriteLine(Text);
            }
        }

    }
}