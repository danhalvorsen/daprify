using Daprify.Models;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace Daprify.Services
{
    public static class DirectoryService
    {
        const string DAPR = "Dapr";

        public static DirectoryInfo FindDirectoryUpwards(string directoryName)
        {
            DirectoryInfo executingDir = new(Assembly.GetExecutingAssembly().Location);
            while (executingDir.Name != directoryName)
            {
                executingDir = executingDir.Parent ?? throw new DirectoryNotFoundException($"Could not find directory: {directoryName}");
            }
            return executingDir;
        }

        public static MyPath SetDirectory(string dirPath)
        {
            MyPath baseDir = CheckProjectType(GetCurrentDirectory());
            MyPath workingDir = MyPath.Combine(baseDir.ToString(), DAPR, dirPath);

            Log.Verbose("Creating working directory, if not already existing: {workingDir}", workingDir);
            Directory.CreateDirectory(workingDir.ToString());
            return workingDir;
        }

        public static MyPath CheckProjectType(IPath dirPath)
        {
            Log.Verbose("Checking project type...");
            string? isTestProject = Environment.GetEnvironmentVariable("isTestProject");
            if (isTestProject == "true")
            {
                Log.Verbose("Program is running from a test project.");
                return GetCurrentDirectory();
            }
            else
            {
                Log.Verbose("Program is not a test project.");
                return GetGitRootDirectory(dirPath);
            }
        }

        /// <summary>
        /// Retrieves the root directory of the Git repository.
        /// If no Git repository is found, the parent directory of the current directory is returned.
        /// </summary>
        /// <returns>The root directory of the Git repository.</returns>
        public static MyPath GetGitRootDirectory(IPath dirPath)
        {
            Log.Verbose("Getting git root directory...");
            string startingPath = Path.GetDirectoryName(dirPath.ToString()) ?? throw new DirectoryNotFoundException($"Could not find the directory: {dirPath}");
            string gitRoot = RunGitCommand(startingPath);

            if (string.IsNullOrWhiteSpace(gitRoot))
            {
                Log.Warning("No Git root directory found.");
                var dir = Directory.GetParent(GetCurrentDirectory().ToString())!;
                Log.Verbose("Setting working directory: {dir}", Path.GetFullPath(dir.ToString()));
                return new(dir.FullName);
            }
            else
            {
                Log.Verbose("Git root directory found: {gitRoot}", gitRoot);
                return new(gitRoot.Trim());
            }
        }

        private static string RunGitCommand(string workingDirectory)
        {
            Log.Verbose("Starting search from {workingDirectory}", workingDirectory);
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --show-toplevel",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode == 0 ? output : string.Empty;
        }

        public static MyPath CreateTempDirectory(IEnumerable<IPath>? paths = null)
        {
            MyPath tempPath = new(Path.GetTempPath());
            tempPath = paths?.Aggregate(tempPath, (current, next) => MyPath.Combine(current.ToString(), next.ToString())) ?? tempPath;

            Directory.CreateDirectory(tempPath.ToString());
            return tempPath;
        }


        public static void DeleteDirectory(IPath directoryPath)
        {
            if (Directory.Exists(directoryPath.ToString()))
            {
                Directory.Delete(directoryPath.ToString(), true);
            }
        }

        public static void WriteFile(IPath workingDir, string fileName, string content)
        {
            MyPath filePath = MyPath.Combine(workingDir.ToString(), fileName);
            Log.Verbose("Writing content to file: {filePath}", filePath);
            File.WriteAllText(filePath.ToString(), content);
            Log.Verbose("Content successfully written to file.");
        }

        public static void AppendFile(IPath workingDir, string fileName, string content)
        {
            MyPath filePath = MyPath.Combine(workingDir.ToString(), fileName);
            Log.Verbose("Appending content to existing file: {filePath}", filePath);
            File.AppendAllText(filePath.ToString(), content);
            Log.Verbose("Content successfully appended to existing file.");
        }

        public static MyPath GetCurrentDirectory()
        {
            return new(Directory.GetCurrentDirectory());
        }
    }
}