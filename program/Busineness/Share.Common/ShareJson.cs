using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Common.Unit
{
    public class ShareJson
    {
        public string EurekaMasterNodeIp { get; set; }
        public int EurekaMasterNodePort { get; set; }



        private static ShareJson _instance;
        public static ShareJson Inst
        {
            get
            {
                if (_instance == null)
                    _instance = From();
                return _instance;
            }
        }
        private const string shareJsonPath = @"Config/Share.json";

        public static ShareJson From()
        {
            return From(shareJsonPath);
        }

        public static ShareJson From(string path)
        {
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ShareJson>(text);
        }
    }
}
