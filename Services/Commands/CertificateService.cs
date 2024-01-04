using System.Diagnostics;
using Daprify.Models;
using Serilog;

namespace Daprify.Services
{

    public class CertificateService() : CommandService(WORKING_DIR)
    {
        private const string WORKING_DIR = "Certs";
        private readonly Name genCert = new("gen_cert.sh");
        private readonly Name readWriteEnv = new("read_write_env.sh");

        public override void Generate(OptionDictionary options)
        {
            PrintExecuting();

            IPath certDir = DirectoryService.SetDirectory(WORKING_DIR);
            Process certProcess = GetProcess(certDir, genCert);
            Process envProcess = GetProcess(certDir, readWriteEnv);

            StartProcess(certProcess);
            CreateEnvFile(certProcess, envProcess);
        }


        private static Process GetProcess(IPath certDir, Name scriptName)
        {
            Log.Verbose("Getting process for script: {scriptName}...", scriptName);

            MyPath scriptPath = BuildScriptPath(scriptName);
            string arguments = BuildArguments(certDir, scriptPath);
            Process process = CreateProcess(arguments);

            Log.Verbose("Process created");
            return process;
        }

        private static MyPath BuildScriptPath(Name scriptName)
        {
            const string SCRIPT_DIR_NAME = "Scripts";
            DirectoryInfo cliDir = DirectoryService.FindDirectoryUpwards("daprify");
            MyPath scriptPath = MyPath.Combine(cliDir.FullName, SCRIPT_DIR_NAME, scriptName.ToString());

            Log.Verbose("Script path: {scriptPath}", scriptPath);
            return scriptPath;
        }

        private static string BuildArguments(IPath certDir, IPath scriptPath)
        {
            string arguments = $"{scriptPath} {certDir}";
            Log.Verbose("Script arguments: {arguments}", arguments);
            return arguments;
        }

        private static Process CreateProcess(string arguments)
        {
            Log.Verbose("Creating script process...");
            var startInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return new Process { StartInfo = startInfo };
        }

        private static void StartProcess(Process process)
        {
            Log.Verbose("Starting script process...");
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += (sender, e) => CollectErrorData(e);
            process.WaitForExit();
        }

        private static void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }

        private static void CollectErrorData(DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine(e.Data);
            }
        }

        private static void CreateEnvFile(Process certProcess, Process envProcess)
        {
            if (certProcess.ExitCode == 0)
            {
                PrintSuccess("certificates");
                Log.Verbose("Starting to create environment file");

                StartProcess(envProcess);

                if (envProcess.ExitCode == 0)
                {
                    PrintSuccess("environmental file");
                }
            }
        }

        private static void PrintSuccess(string name)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Successfully created {name}");
            Console.ResetColor();
        }
    }
}
