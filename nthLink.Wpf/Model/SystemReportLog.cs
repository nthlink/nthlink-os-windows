using nthLink.Header.Enum;
using nthLink.Header.Interface;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace nthLink.Wpf.Model
{
    class SystemReportLog : ISystemReportLog
    {
        private const string PATH = "SystemLog";
        private const int STRING_BUILDER_CAPACITY = 500 * 10000;
        private StringBuilder stringBuilder = new StringBuilder(STRING_BUILDER_CAPACITY);

        private string? currentDirectory = string.Empty;
        private string logFileDirectory = string.Empty;

        public SystemReportLog()
        {
            Process currentProcess = Process.GetCurrentProcess();

            if (currentProcess.MainModule != null)
            {
                string? processPath = currentProcess.MainModule.FileName;

                string? processDirectory = System.IO.Path.GetDirectoryName(processPath);

                this.currentDirectory = processDirectory;
            }
            this.logFileDirectory = string.IsNullOrEmpty(this.currentDirectory) ?
                PATH : Path.Combine(this.currentDirectory, PATH);
            if (!Directory.Exists(this.logFileDirectory))
            {
                Directory.CreateDirectory(this.logFileDirectory);
            }
        }

        public async Task Log(LogLevelEnum logLevel, string message)
        {
            if ((this.stringBuilder.Length + message.Length) > this.stringBuilder.Capacity)
            {
                await Save();
            }

            this.stringBuilder.AppendLine(
                $"[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}] [{logLevel.ToString()}] {message}"
                );
        }

        public async Task Save()
        {
            if (this.stringBuilder.Length > 0)
            {
                await LogToFile(this.stringBuilder.ToString());

                this.stringBuilder = new StringBuilder(STRING_BUILDER_CAPACITY);
            }
        }
        public async Task<string> LogToFile(string log)
        {
            if (log.Length > 0)
            {
                string fileFullPath;

                string fileName = $"{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")}.txt";

                if (string.IsNullOrEmpty(this.currentDirectory))
                {
                    fileFullPath = fileName;
                }
                else
                {
                    string filePath = this.logFileDirectory;

                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        fileFullPath = Path.Combine(filePath, fileName);
                    }
                    catch
                    {
                        fileFullPath = fileName;
                    }
                }

                await File.WriteAllTextAsync(fileFullPath, log);

                return fileFullPath;
            }

            return string.Empty;
        }

        public async Task<string> LogToFile(byte[] data)
        {
            if (data.Length > 0)
            {
                string fileFullPath;

                string fileName = $"{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff")}.dat";

                if (string.IsNullOrEmpty(this.currentDirectory))
                {
                    fileFullPath = fileName;
                }
                else
                {
                    string filePath = this.logFileDirectory;

                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        fileFullPath = Path.Combine(filePath, fileName);
                    }
                    catch
                    {
                        fileFullPath = fileName;
                    }
                }

                await File.WriteAllBytesAsync(fileFullPath, data);

                return fileFullPath;
            }

            return string.Empty;
        }

        public Task ClearLog(TimeSpan timeSpan)
        {
            return Task.Run(() =>
            {
                if (timeSpan != TimeSpan.Zero)
                {
                    string filePath = this.logFileDirectory;

                    DateTime time = DateTime.Now - timeSpan;

                    foreach (string file in Directory.GetFiles(filePath))
                    {
                        FileInfo fileInfo = new FileInfo(file);

                        if ((fileInfo.Extension.EndsWith("txt") || fileInfo.Extension.EndsWith("dat")) &&
                            fileInfo.LastWriteTime < time)
                        {
                            try
                            {
                                fileInfo.Delete();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            });
        }
    }
}
