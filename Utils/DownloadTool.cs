using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HxxServerDownloader.Utils
{
    public class DownloadTool
    {
        public FileDownloader Downloader { get; set; } = new FileDownloader();

        public async Task Download(string rawUrl, DownloadResourse resourse, string uid)
        {
            try
            {
                string destinationPath = string.Empty;
                string extractPath = string.Empty;

                switch (resourse)
                {
                    case DownloadResourse.Server:
                        destinationPath = $"./servers/{uid}/{uid}.zip";
                        extractPath = $"./servers/{uid}";
                        Directory.CreateDirectory(extractPath);
                        break;
                    case DownloadResourse.Mods:
                        extractPath = Path.Combine(Directory.GetDirectories($"./servers/{uid}/.minecraft/versions")[0], "mods");
                        clearDirectory(extractPath);
                        destinationPath = Path.Combine(extractPath, "mods.zip");
                        break;
                    case DownloadResourse.Configs:
                        extractPath = Path.Combine(Directory.GetDirectories($"./servers/{uid}/.minecraft/versions")[0], "config");
                        clearDirectory(extractPath);
                        destinationPath = Path.Combine(extractPath, "configs.zip");
                        break;
                }

                await Downloader.DownloadAndExtractAsync(rawUrl, destinationPath, extractPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"下载失败: {ex.Message}\n{ex.StackTrace}", "下载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void clearDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                File.Delete(file);
            }

            foreach (var subDirectory in Directory.GetDirectories(directoryPath))
            {
                Directory.Delete(subDirectory, true);
            }
        }
    }

    public enum DownloadResourse
    {
        Server,
        Mods,
        Configs
    }

    public class FileDownloader
    {
        private readonly HttpClient _httpClient;

        public event Action<int> ProgressChanged;

        public FileDownloader()
        {
            _httpClient = new HttpClient();
        }

        public async Task DownloadAndExtractAsync(string url, string destinationPath, string extractPath)
        {
            await Task.Run(async () =>
            {
                try
                {
                    using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        var canReportProgress = totalBytes != -1;

                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var totalRead = 0L;
                            var buffer = new byte[8192];
                            var isMoreToRead = true;

                            do
                            {
                                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                    TriggerProgressChanged(100); // 下载完成，进度增加到50%
                                    continue;
                                }

                                await fileStream.WriteAsync(buffer, 0, read);

                                totalRead += read;
                                if (canReportProgress)
                                {
                                    var progress = (int)((totalRead * 100) / totalBytes); // 下载进度占100%
                                    TriggerProgressChanged(progress);
                                }
                            } while (isMoreToRead);

                            MessageBox.Show("下载完成，请等待解压完成。", "下载完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"下载失败，请检查网络连接或稍后再试: {ex.Message}\n{ex.StackTrace}", "下载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            await Task.Run(() =>
            {
                try
                {
                    // 确保解压路径存在
                    if (!Directory.Exists(extractPath))
                    {
                        Directory.CreateDirectory(extractPath);
                    }
                
                    var p = Process.Start(".\\bz.exe", $"x -o:\"{extractPath}\" -y \"{destinationPath}\"");
                    p.WaitForExit();
                    MessageBox.Show("解压完成。", "解压完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"解压失败: {ex.Message}\n{ex.StackTrace}", "解压失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            File.Delete(destinationPath); // 删除下载的 zip 文件
        }

        private void TriggerProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(progress);
        }
    }
}
