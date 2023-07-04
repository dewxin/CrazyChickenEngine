using Block.RPC.Task;
using Server.Game.ServiceWorld.Manager;
using Block.Assorted.Logging;
using Share.IService.Service.World;
using Share.IService.Param;

namespace GameServerBase.ServerWorld
{
    public class Client2WorldHandler : GameServiceHandler, IClient2World
    {
        private WorldApplication WorldService => Application as WorldApplication;

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
                //Log.Warn($"already in match Id{playerInfo.Id} {playerInfo.Name}");
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
