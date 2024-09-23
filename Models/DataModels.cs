using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HxxServerDownloader.Models
{
    public class DataModels
    {
        public class ServerItem
        {
            public string Name { get; set; }
            public string Uid { get; set; }
        }

        public class DataModel
        {
            public List<ServerItem> Servers { get; set; } = new List<ServerItem>();
        }
    }
}
