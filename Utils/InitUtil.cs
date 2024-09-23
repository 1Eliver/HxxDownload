using HxxServerDownloader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HxxServerDownloader.Utils
{
    public static class InitUtil
    {
        public static void InitData()
        {
            if (File.Exists("./data.json")) return;
            File.WriteAllText("./data.json", JsonConvert.SerializeObject(new DataModels.DataModel()));
        }

        public static void InitServerFolder()
        {
            Directory.CreateDirectory("./servers");
        }
    }
}
