using LiteDB;
using System;

namespace EasyTools.DataStructures
{
    [Serializable]
    public class PlayerData
    {
        public string NickName { get; set; }
        public DateTime LastJoinedTime { get; set; }
        public DateTime LastLeftTime { get; set; }
        public int PlayedTimes { get; set; }
        public int PlayerKills { get; set; }
        public int PlayerDeath { get; set; }
        public int PlayerSCPKills { get; set; }
        public float PlayerDamage { get; set; }
        public int RolePlayed { get; set; }
        public int PlayerShot { get; set; }
        public double PlayerXp { get; set; }
        public double PlayerLevel { get; set; }
        public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Player;
        public string Badge { get; set; } = "";
        public string BadgeColor { get; set; } = "rainbow";
        [BsonId]
        public string ID { get; set; }
    }

    public enum PermissionLevel
    {
        Player = 0,
        Moderator = 1,
        Admin = 2,
        Owner = 3
    }
}
