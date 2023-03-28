using Block.Assorted;
using Block.Server.Common;
using Block0.Net;
using Block1.LocatableRPC;
using Chunk.LocatableRPC;
using Server.Common.Unit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Server.Common
{
    public interface IServerInfoGetter
    {
        ServerNode GetServer();
        SocketConfig GetSocketConfig(); 
        List<ServerService> GetServiveList();
    }

    public sealed class GlobalConfig
    {
        public static GlobalConfig Inst = new GlobalConfig();
        public IPEndPoint EurekaMasterIPEndPoint { get; set; }
    }

    public interface IGlobalInfoGetter
    {
        GlobalConfig GetGlobalConfig();
    }


    public class AllConfig
    {
        public GlobalConfig GlobalConfig { get; set; }
        public ServerNode ServerNode { get; set; }
        public SocketConfig SocketConfig { get; set; }
        public List<ServerService> ServiceList { get; set; }
    }

    public static class ServerStartHelper
    {
        private static IGetter NewGetter<IGetter>(Assembly assembly)
            where IGetter : class
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(IGetter)))
                {
                    var serverInfoGetter = Activator.CreateInstance(type) as IGetter;
                    return serverInfoGetter;
                }
            }
            return null;
        }

        public static AllConfig LoadNecessaryLib(Assembly assemblyWhereGetterBelong)
        {
            var serverInfoGetter = NewGetter<IServerInfoGetter>(assemblyWhereGetterBelong);
            var commonInfoGetter = NewGetter<IGlobalInfoGetter>(assemblyWhereGetterBelong);

            return new AllConfig
            {
                GlobalConfig = commonInfoGetter.GetGlobalConfig(),
                ServerNode = serverInfoGetter.GetServer(),
                SocketConfig = serverInfoGetter.GetSocketConfig(),
                ServiceList = serverInfoGetter.GetServiveList(),
            };
        }

        public static AllConfig GetAllConfigFromFiles(IEnumerable<string> fileNameList)
        {
            List<string> filePathList = new List<string>();
            fileNameList.ToList().ForEach(t => filePathList.Add(Path.Combine("_ServerGetter", $"{t}.cs")));
            var type = Type.GetType("Server.Game._ServerGetter.GlobalInfoGetter, Server.Game");
            Debug.Assert(type != null);
            LoadNecessaryLib(type.Assembly);



            List<string> codeList = new List<string>();

            filePathList.ToList().ForEach(filepath => codeList.Add(File.ReadAllText(filepath)));

            var assembly = RoslynCompiler.GenerateAssembly(codeList);

            var serverInfoGetter = NewGetter<IServerInfoGetter>(assembly);
            var commonInfoGetter = NewGetter<IGlobalInfoGetter>(assembly);

            return new AllConfig
            {
                GlobalConfig = commonInfoGetter.GetGlobalConfig(),
                ServerNode = serverInfoGetter.GetServer(),
                SocketConfig = serverInfoGetter.GetSocketConfig(),
                ServiceList = serverInfoGetter.GetServiveList(),
            };

        }

    }


}
