using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Common.Unit
{
    //TODO 找个办法把json文件自动拷贝到客户端和服务端的对应位置。最好是使用公共组件实现。
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
            string dir = Directory.GetCurrentDirectory();
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ShareJson>(text);
        }
    }
}
