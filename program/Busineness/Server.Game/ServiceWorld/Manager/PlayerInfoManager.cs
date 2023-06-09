﻿using Block.Assorted;
using Block.Assorted.Logging;
using System;
using System.Collections.Generic;


namespace Server.Game.ServiceWorld.Manager
{

    internal class PlayerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    internal class PlayerInfoManager
    {

        private IDGenerator IdGenerator = new IDGenerator(1);
        private Dictionary<string, PlayerInfo> playerName2InfoDict = new Dictionary<string, PlayerInfo>();


        public bool NameIsTaken(string name)
        {
            return playerName2InfoDict.ContainsKey(name);
        }

        public bool RegisterPlayer(string name, out PlayerInfo? playerInfo)
        {
            if(NameIsTaken(name))
            {
                Log.Warn($"name is taken:{name}");
                playerInfo = null;
                return false;
            }


            playerInfo = new PlayerInfo { Id = IdGenerator.GetIntID(), Name = name };
            playerName2InfoDict.Add(name, playerInfo);
            return true;
        }
    }
}
