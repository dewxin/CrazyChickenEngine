using Block.RPC.Attr;
using Block.RPC.Task;
using Share.IService.Param;
using System;

namespace Share.IService.Service.World
{
    public interface IClient2World : IWorldService
    {
        MethodCallTask<WorldLoginRet> Login(WorldLoginData worldLoginData);

        MethodCallTask<MatchResult> EnqueueForMatch(PlayerMatchRequest playerData);
    }
}
