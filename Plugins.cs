using EasyTools.Configs;
using EasyTools.DataBase.Serialization;
using EasyTools.Events;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System.IO;

namespace EasyTools
{
    public class Plugins : Plugin
    {
        public CustomEventHandler Events { get; } = new();
        public override void LoadConfigs()
        {
            base.LoadConfigs();

            CustomEventHandler.Config = this.LoadConfig<Config>("config.yml");
            CustomEventHandler.TranslateConfig = this.LoadConfig<TranslateConfig>("translateConfig.yml");
            CustomEventHandler.BadgeConfig = this.LoadConfig<BadgeConfig>("badgeConfig.yml");
            CustomEventHandler.CustomRoleConfig = this.LoadConfig<CustomRoleConfig>("customRoleConfig.yml");
            CustomEventHandler.DataBaseConfig = this.LoadConfig<DataBaseConfig>("dataBaseConfig.yml");
            CustomEventHandler.HUDInfoConfig = this.LoadConfig<HUDInfoConfig>("HUDInfoConfig.yml");

            CustomEventHandler.data_914 = new HintData
            (
                CustomEventHandler.HUDInfoConfig._914_x,
                CustomEventHandler.HUDInfoConfig._914_y,
                CustomEventHandler.HUDInfoConfig._914_font
            );

            CustomEventHandler.data_elevator = new HintData
            (
                CustomEventHandler.HUDInfoConfig._elev_x,
                CustomEventHandler.HUDInfoConfig._elev_y,
                CustomEventHandler.HUDInfoConfig._elev_font
            );

            if (!Directory.Exists(CustomEventHandler.BadgeConfig.Pach))
            {
                Directory.CreateDirectory(CustomEventHandler.BadgeConfig.Pach);
            }
        }


        public static System.Version RequiredGameVersion => new(14, 2, 1);

        public static Plugins Instance { get; private set; }

        public override string Name => "EasyTools";

        public override string Description => "一个简单的服务器工具插件";

        public override string Author => "3cxc";

        public override System.Version Version => new(1, 0, 1);

        public override System.Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Instance = this;

            CustomHandlersManager.RegisterEventsHandler(Events);
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);

            Instance = null;

            foreach (var hud in CustomEventHandler._huds.Values) hud.Dispose();
            CustomEventHandler._huds.Clear();
        }

    }
}
