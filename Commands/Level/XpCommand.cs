using CommandSystem;
using EasyTools.Configs;
using EasyTools.DataBase;
using EasyTools.DataBase.Serialization;
using EasyTools.Events;
using EasyTools.Utils;
using LabApi.Features.Wrappers;
using System;

namespace EasyTools.Commands.Level
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class XpCommand : ICommand
    {
        public string Command => "xp";

        public string[] Aliases => [];

        public string Description => "查询经验";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            TranslateConfig TranslateConfig = CustomEventHandler.TranslateConfig;
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = TranslateConfig.RescueCommandError;
                return false;
            }

            if (!CustomEventHandler.DataBaseConfig.database_enable)
            {
                response = TranslateConfig.LevelCommandFailed;
                return false;
            }

            PlayerData data = player.GetData();
            double xp = data.PlayerXp;
            int level = LevelUtils.GetLevelFromXp(xp, 1);
            double needXp = LevelUtils.GetXpUntilNextLevel(xp, 1);

            response = $"你的等级: {level} | 当前经验: {xp} | 升级还需: {needXp} XP";
            return true;
        }
    }
}
