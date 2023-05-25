using Block.RPC.Task;
using Share.IService.Param;
using System;
using System.Collections.Generic;

namespace Server.Game.ServiceWorld.Manager
{
    internal class MatchManager
    {
        private Queue<PlayerInfo> playerQueue = new Queue<PlayerInfo>();
        private Queue<MatchTask> taskQueue = new Queue<MatchTask>();
        private HashSet<int> playerIdInQueue = new HashSet<int>();


        public bool StartMatchTask(PlayerInfo playerInfo, out MatchTask matchTask)
        {
            if(playerIdInQueue.Contains(playerInfo.Id))
            {
                matchTask = null;
                return false;
            }

            matchTask = new MatchTask();
            taskQueue.Enqueue(matchTask);
            playerIdInQueue.Add(playerInfo.Id);
            playerQueue.Enqueue(playerInfo);

            return true;
        }


        public PlayerInfo Dequeue()
        {
            var playerInfo = playerQueue.Dequeue();
            playerIdInQueue.Remove(playerInfo.Id);

            return playerInfo;
        }


        public void CheckTaskState()
        {
            if(playerQueue.Count == 2)
            {
                var playerInfo1 = Dequeue();
                var playerInfo2 = Dequeue();

                MatchResult matchResult = new MatchResult
                {
                    MatchSucceed= true,
                    PlayerData = new List<PlayerMatchRet>
                    {
                        new PlayerMatchRet() { Id = playerInfo1.Id, Name = playerInfo1.Name, Party = PlayerMatchParty.Red },
                        new PlayerMatchRet(){Id = playerInfo2.Id, Name = playerInfo2.Name, Party = PlayerMatchParty.Blue},
                    }
                };


                var task = taskQueue.Dequeue();
                task.MatchResult = matchResult;
                task.OnFinish();

                var task2 = taskQueue.Dequeue();
                task2.MatchResult= matchResult;
                task2.OnFinish();
            }
        }


    }


    internal class MatchTask:BaseTask
    {
        internal MatchResult MatchResult { get; set; }
    }

}
