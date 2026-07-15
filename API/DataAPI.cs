using EasyTools.DataStructures;
using EasyTools.Events;
using LiteDB;
using System.Collections.Generic;

namespace EasyTools.API
{
    public static class DataAPI
    {
        public static List<string> TimerHidden { get; } = [];
        public static Dictionary<string, PlayerData> PlayerDataDic = [];

        public static bool TryGetData(string id, out PlayerData data)
        {
            if (PlayerDataDic.TryGetValue(id, out data))
            {
                return true;
            }
            using LiteDatabase database = new(CustomEventHandler.Config.DataBasePath);

            if ((data = database.GetCollection<PlayerData>("Players")?.FindById(id)) != null)
            {
                PlayerDataDic.Add(id, data);
                return true;
            }
            return false;
        }
    }
}
