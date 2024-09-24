using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HxxServerDownloader.Utils
{
    public class FilesUtils
    {
        public class DirectoryMover
        {
            public event Action<int> ProgressChanged; // 修改为整数

            public void MoveDirectory(string sourcePath, string destinationPath)
            {
                if (!Directory.Exists(sourcePath))
                    throw new DirectoryNotFoundException($"Source path not found: {sourcePath}");

                Directory.CreateDirectory(destinationPath);

                var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                var totalFiles = files.Length;
                var movedFiles = 0;

                foreach (var file in files)
                {
                    var relativePath = GetRelativePath(sourcePath, file);
                    var destinationFile = Path.Combine(destinationPath, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                    File.Move(file, destinationFile);

                    movedFiles++;
                    int progress = (int)Math.Floor((double)movedFiles / totalFiles * 100); // 计算整数进度
                    ProgressChanged?.Invoke(progress);
                }

                ProgressChanged?.Invoke(100);
            }

            private string GetRelativePath(string basePath, string fullPath)
            {
                Uri baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? basePath : basePath + Path.DirectorySeparatorChar);
                Uri fullUri = new Uri(fullPath);
                return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
            }
        }

        public static class PathUtils
        {
            public static string GetServerPath(string uid)
            {
                return Path.Combine("./servers", uid);
            }
        }
    }
}
