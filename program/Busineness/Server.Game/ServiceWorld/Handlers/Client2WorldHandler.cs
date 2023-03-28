using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Service;
using Protocol.Param;
using Protocol.Service.World;
using Protocol.Service.Login;
using Block.RPC.Task;
using Server.Game.ServiceWorld.Manager;
using System.Diagnostics;
using Block.Assorted.Logging;

namespace GameServerBase.ServerWorld
{
    public class Client2WorldHandler : GameServiceHandler, IClient2World
    {
        private WorldService WorldService => GameService as WorldService;

        public Client2WorldHandler()
        {
        }

        public MethodCallTask<MatchResult> EnqueueForMatch(PlayerMatchRequest playerData)
        {
            var playerInfo = new PlayerInfo()
            {
                Id= playerData.Id,
                Name= playerData.Name,
            };
            Log.Debug($"id={playerInfo.Id}, name={playerInfo.Name}");

            var startMatchSucceed = WorldService.MatchManager.StartMatchTask(playerInfo, out var matchTask);

            if(!startMatchSucceed)
            {
                Log.Info($"fail");
                return new MatchResult { MatchSucceed = false, };
            }


            var retTask = new MethodCallTask<MatchResult>();
            retTask.Action = () =>
            {
                retTask.MethodCallResult = matchTask.MatchResult;
            };

            retTask.Wait(matchTask);

            return retTask;

        }

        public MethodCallTask<WorldLoginRet> Login(WorldLoginData worldLoginData)
        {
            var succeed = WorldService.PlayerInfoManager.RegisterPlayer(worldLoginData.Name, out var playerInfo);

            if (succeed)
            {
                return new WorldLoginRet()
                { 
                    PlayerId = playerInfo.Id,
                    PlayerName = playerInfo.Name,
                    Result = WorldLoginResult.Success 
                };
            }
            else
            {
                return new WorldLoginRet
                {
                    Result = WorldLoginResult.Err_NameDuplicate,
                };

            }

        }
    }
}
