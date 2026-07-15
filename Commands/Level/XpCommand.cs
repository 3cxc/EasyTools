using CommandSystem;
using EasyTools.Configs;
using EasyTools.DataStructures;
using EasyTools.Events;
using EasyTools.Extensions;
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
                response = TranslateConfig.CommandFailed;
                return false;
            }

            if (!CustomEventHandler.LevelSystemConfig.EnableLevelSystem)
            {
                response = CustomEventHandler.TranslateConfig.CommandNotEnabled;
                return false;
            }

            PlayerData data = player.GetData();
            double xp = data.PlayerXp;
            int level = LevelExtensions.GetLevelFromXp(xp, CustomEventHandler.LevelSystemConfig.XpScaleFactor);
            double needXp = LevelExtensions.GetXpUntilNextLevel(xp, CustomEventHandler.LevelSystemConfig.XpScaleFactor);

            response = $"你的等级: {level} | 当前经验: {xp} | 升级还需: {needXp} XP";
            return true;
        }
    }
}
