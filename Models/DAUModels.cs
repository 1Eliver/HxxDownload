using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HxxServerDownloader.Models
{
    using Newtonsoft.Json;

    public class DAUModels
    {
        public class Download
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("uid")]
            public string Uid { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }

            public override string ToString()
            {
                return $"Name: {Name}, Uid: {Uid}, Url: {Url}";
            }
        }

        public class Mods
        {
            public string Url { get; set; }

            public override string ToString()
            {
                return $"Url: {Url}";
            }
        }

        public class Configs
        {
            public string Url { get; set; }

            public override string ToString()
            {
                return $"Url: {Url}";
            }
        }

        public class Upload
        {
            [JsonProperty("uid")]
            public string Uid { get; set; }
            [JsonProperty("mods")]
            public Mods Mods { get; set; }
            [JsonProperty("configs")]
            public Configs Configs { get; set; }

            public override string ToString()
            {
                return $"Uid: {Uid}, Mods: [{Mods}], Configs: [{Configs}]";
            }
        }

        public class DownloadAndUploadData
        {
            [JsonProperty("download")]
            public Download Download { get; set; }
            [JsonProperty("upload")]
            public Upload Upload { get; set; }

            public override string ToString()
            {
                return $"Download: [{Download}], Upload: [{Upload}]";
            }
        }
    }

}
