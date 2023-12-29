using System.Diagnostics;
using System.Text;
using CLI.Models;

namespace CLI.Services
{

    public class CertificateService() : CommandService(WORKING_DIR)
    {
        private const string WORKING_DIR = "Certs";
        private readonly Name genCert = new("gen_cert.sh");
        private readonly Name readWriteEnv = new("read_write_env.sh");

        public override void Generate(OptionDictionary options)
        {
            StringBuilder errorOutput = new();

            Process certProcess = GetProcess(genCert);
            Process envProcess = GetProcess(readWriteEnv);

            StartProcess(certProcess, errorOutput);
            certProcess.WaitForExit();

            if (certProcess.ExitCode == 0)
            {
                StartProcess(envProcess, errorOutput);
                envProcess.WaitForExit();
            }
        }

        private static Process GetProcess(Name scriptName)
        {
            IPath certDir = DirectoryService.SetDirectory(WORKING_DIR);

            MyPath scriptPath = BuildScriptPath(scriptName);
            string arguments = BuildArguments(certDir, scriptPath);
            Process process = CreateProcess(arguments);

            return process;
        }

        private static MyPath BuildScriptPath(Name scriptName)
        {
            const string SCRIPT_DIR_NAME = "Scripts";
            DirectoryInfo cliDir = DirectoryService.FindDirectoryUpwards("daprify");

            return MyPath.Combine(cliDir.FullName, SCRIPT_DIR_NAME, scriptName.ToString());
        }

        private static string BuildArguments(IPath certDir, IPath scriptPath)
        {
            return $"{scriptPath} {certDir}";
        }

        private static Process CreateProcess(string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return new Process { StartInfo = startInfo };
        }

        private static void StartProcess(Process process, StringBuilder errorOutput)
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += (sender, e) => CollectErrorData(e, errorOutput);
        }

        private static void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }

        private static void CollectErrorData(DataReceivedEventArgs e, StringBuilder errorOutput)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorOutput.AppendLine(e.Data);
                Console.WriteLine(e.Data);
            }
        }
    }
}
