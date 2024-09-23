using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HxxServerDownloader.Utils
{
    public static class DecodeUtil
    {
        public static string Decode(string base64EncodedData)
        {
            byte[] compressedData = Convert.FromBase64String(base64EncodedData);
            using (var compressedStream = new MemoryStream(compressedData))
            using (var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressedStream, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
