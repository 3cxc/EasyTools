using System.ComponentModel;

namespace EasyTools.Configs
{
    public class BadgeConfig
    {
        [Description("是否启用称号系统")]
        public bool Enable { get; set; } = true;
        [Description("彩色称号更新频率")]
        public int Each { get; set; } = 1;
    }
}
