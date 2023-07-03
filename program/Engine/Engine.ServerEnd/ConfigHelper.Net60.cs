using Block.Assorted;
using Block0.Net;
using Block1.LocatableRPC;
using Engine.Common.Unit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Engine.ServerEnd
{

    public static class ConfigHelper
    {

        //TODO 返回list?
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
                ApplicationList = serverInfoGetter.GetApplicationList(),
            };
        }

        public static AllConfig GetAllConfigFromFiles(Type getterType,IEnumerable<string> filePathList)
        {
            Debug.Assert(getterType != null);
            LoadNecessaryLib(getterType.Assembly);


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
                ApplicationList = serverInfoGetter.GetApplicationList(),
            };

        }

    }

}

