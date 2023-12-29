using System.Diagnostics;
using System.Text;
using CLI.Models;

namespace CLI.Services
{

    public class CertificateService() : CommandService(WORKING_DIR)
    {
        private const string WORKING_DIR = "Certs";
        private const string GEN_CERT_SH = "gen_cert.sh";
        private const string READ_WRITE_ENV = "read_write_env.sh";

        public override void Generate(OptionDictionary options)
        {
            StringBuilder errorOutput = new();

            Process certProcess = GetProcess(GEN_CERT_SH);
            Process envProcess = GetProcess(READ_WRITE_ENV);

            StartProcess(certProcess, errorOutput);
            certProcess.WaitForExit();

            if (certProcess.ExitCode == 0)
            {
                StartProcess(envProcess, errorOutput);
                envProcess.WaitForExit();
            }
        }

        private static Process GetProcess(string scriptName)
        {
            IPath certDir = DirectoryService.SetDirectory(WORKING_DIR);

            string scriptPath = BuildScriptPath(scriptName);
            string arguments = BuildArguments(certDir, scriptPath);
            Process process = CreateProcess(arguments);

            return process;
        }

        private static string BuildScriptPath(string scriptName)
        {
            const string SCRIPT_DIR_NAME = "Scripts";
            DirectoryInfo cliDir = DirectoryService.FindDirectoryUpwards("daprify");

            return Path.Combine(cliDir.FullName, SCRIPT_DIR_NAME, scriptName);
        }

        private static string BuildArguments(IPath certDir, string scriptPath)
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
