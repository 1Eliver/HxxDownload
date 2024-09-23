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
    public class DataManager
    {
        public DataModels.DataModel Data { get; set; }

        public DataManager()
        {
            Load();
        }

        private void Load()
        {
            InitUtil.InitData();
            Data = JsonConvert.DeserializeObject<DataModels.DataModel>(
                File.ReadAllText("./data.json")
                );
        }

        public void Save()
        {
            File.WriteAllText("./data.json", JsonConvert.SerializeObject(Data));
        }
    }
}
