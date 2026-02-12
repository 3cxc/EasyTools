using EasyTools.DataBase.Serialization;
using EasyTools.Events;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;

namespace EasyTools.DataBase
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
            using LiteDatabase database = new(CustomEventHandler.DataBaseConfig.database_path);
            
            if ((data = database.GetCollection<PlayerData>("Players")?.FindById(id)) != null)
            {
                PlayerDataDic.Add(id, data);
                return true;
            }
            return false;
        }
    }
}
