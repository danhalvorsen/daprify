using System.Diagnostics;
using System.Reflection;

namespace CLI.Services
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

        public static string SetDirectory(string dirPath)
        {
            string baseDir = CheckProjectType();
            string workingDir = Path.Combine(baseDir, DAPR, dirPath);
            Directory.CreateDirectory(workingDir);
            return workingDir;
        }

        public static string CheckProjectType()
        {
            string? isTestProject = Environment.GetEnvironmentVariable("isTestProject");
            return isTestProject == "true" ? Directory.GetCurrentDirectory() : GetGitRootDirectory();
        }

        /// <summary>
        /// Retrieves the root directory of the Git repository.
        /// If no Git repository is found, the parent directory of the current directory is returned.
        /// </summary>
        /// <returns>The root directory of the Git repository.</returns>
        public static string GetGitRootDirectory()
        {
            using Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --show-toplevel",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode == 0 ? output.Trim() : Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
        }

        public static string? GetFileInDirectory(string dirPath, string fileType)
        {
            return Directory.GetFiles(dirPath, fileType).FirstOrDefault();
        }

        public static string CreateTempDirectory(List<string>? paths = null)
        {
            string tempPath = Path.GetTempPath();

            if (paths != null)
            {
                tempPath = paths.Aggregate(tempPath, Path.Combine);
            }

            if (!Directory.Exists(tempPath))
            {
                throw new DirectoryNotFoundException("Directory does not exist: " + tempPath);
            }

            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        public static void DeleteDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        public static void WriteFile(string workingDir, string fileName, string content)
        {
            string filePath = Path.Combine(workingDir, fileName);
            File.WriteAllText(filePath, content);
        }

        public static void AppendFile(string workingDir, string fileName, string content)
        {
            string filePath = Path.Combine(workingDir, fileName);
            File.AppendAllText(filePath, content);
        }
    }
}