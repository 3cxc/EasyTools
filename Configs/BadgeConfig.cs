using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTools.Configs
{
    public class BadgeConfig
    {
        [Description("是否启用称号系统")]
        public bool Enable { get; set; } = true;
        [Description("存储路径")]
        public string Pach { get; set; } = "/home/shiroko/.config/DIRSave";
        [Description("彩色称号更新频率")]
        public int each { get; set; } = 1;
    }
}
