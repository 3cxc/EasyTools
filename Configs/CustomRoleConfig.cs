using System.ComponentModel;

namespace EasyTools.Configs
{
    public class CustomRoleConfig
    {
        [Description("开启3114?")]
        public bool spawn_scp_3114 { get; set; } = false;
        [Description("当有多少人时才会生成3114?")]
        public int spawn_scp_3114_limit { get; set; } = 8;
    }
}
