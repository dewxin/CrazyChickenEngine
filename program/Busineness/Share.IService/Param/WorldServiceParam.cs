using System.Collections.Generic;

namespace Share.IService.Param
{

    public class WorldLoginData
    {
        public string Name { get; set; }
    }

    public enum WorldLoginResult : byte
    {
        None = 0,
        Success = 1,
        Err_NameDuplicate = 2,
    }

    public class WorldLoginRet
    {
        public WorldLoginResult Result { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = "";
    }


    public class PlayerMatchRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }


    public enum PlayerMatchParty : byte
    {
        None = 0,
        Red = 1,
        Blue = 2,
    }

    public class PlayerMatchRet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PlayerMatchParty Party { get; set; }
    }


    public class MatchResult
    {
        public bool MatchSucceed { get; set; }
        public int MapId { get; set; }

        public List<PlayerMatchRet> PlayerData { get; set; } = new List<PlayerMatchRet>();
    }


}

#pragma warning restore CS1591, CS0612, CS3021, IDE1006
