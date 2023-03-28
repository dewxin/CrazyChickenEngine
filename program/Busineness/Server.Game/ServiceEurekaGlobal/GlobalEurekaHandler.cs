﻿using Block.RPC.Task;
using Chunk.LocatableRPC;
using Protocol.Param;
using Protocol.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ServiceEurekaGlobal
{

    public class GlobalEurekaHandler : GameServiceHandler, IEurekaMasterService
    {
        //private ServerInfoManager ServerInfoManager => Server.ServerInfoManager;
        public new GlobalEurekaService GameService => base.GameService as GlobalEurekaService;


        public GlobalEurekaHandler()
        {
        }


        //自己也会通过NodeEurekaService请求注册节点信息
        public MethodCallTask<NodePubRet> PubHostInfoAndSub(NodeInfo registerData)
        {
            var hostID = GameService.NodeInfoManager.RegisterNodeRetID(registerData);

            var hostPubRet = new NodePubRet();
            hostPubRet.NodeId = hostID;
            hostPubRet.NodeInfoList = GameService.NodeInfoManager.GetHostList();

            return hostPubRet;
        }


        public MethodCallTask<List<NodeInfo>> GetNodesContainingService(ServiceTypeWrapper serviceTypeEnum)
        {
            return GameService.NodeInfoManager.GetNodesByServiceType(serviceTypeEnum.ServiceType);
        }
    }

}
