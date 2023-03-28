using Block.RPC.Attr;
using Block.RPC.Task;
using Protocol.Param;
using System;

namespace Protocol.Service.World
{
    public interface IClient2World: IWorldService
    {
        MethodCallTask<WorldLoginRet> Login(WorldLoginData worldLoginData);

        MethodCallTask<MatchResult> EnqueueForMatch(PlayerMatchRequest playerData);
    }
}
