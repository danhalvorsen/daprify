using Daprify.Models;
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

            Directory.CreateDirectory(workingDir.ToString());
            return workingDir;
        }

        public static MyPath CheckProjectType(IPath dirPath)
        {
            string? isTestProject = Environment.GetEnvironmentVariable("isTestProject");
            return isTestProject == "true" ? GetCurrentDirectory() : GetGitRootDirectory(dirPath);
        }

        /// <summary>
        /// Retrieves the root directory of the Git repository.
        /// If no Git repository is found, the parent directory of the current directory is returned.
        /// </summary>
        /// <returns>The root directory of the Git repository.</returns>
        public static MyPath GetGitRootDirectory(IPath dirPath)
        {
            string startingPath = Path.GetDirectoryName(dirPath.ToString()) ?? throw new DirectoryNotFoundException($"Could not find the directory: {dirPath}");
            string gitRoot = RunGitCommand(startingPath);
            return string.IsNullOrWhiteSpace(gitRoot) ? new(Directory.GetParent(GetCurrentDirectory().ToString())!.FullName) : new(gitRoot.Trim());
        }

        private static string RunGitCommand(string workingDirectory)
        {
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
            File.WriteAllText(filePath.ToString(), content);
        }

        public static void AppendFile(IPath workingDir, string fileName, string content)
        {
            MyPath filePath = MyPath.Combine(workingDir.ToString(), fileName);
            File.AppendAllText(filePath.ToString(), content);
        }

        public static MyPath GetCurrentDirectory()
        {
            return new(Directory.GetCurrentDirectory());
        }
    }
}