using LabApi.Loader.Features.Paths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTools.Configs
{
    public class DataBaseConfig
    {
        [Description("是否启用玩家数据储存系统")]
        public bool database_enable { get; set; } = true;
        [Description("数据库存储路径")]
        public string database_path { get; set; } = Path.Combine(PathManager.Configs.FullName ?? Environment.CurrentDirectory, @"EasyTools.db");
    }
}
