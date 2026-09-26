using System;
using System.IO;
using Log = LabApi.Features.Console.Logger;

namespace EasyTools.Logger
{
    public static class FileLogger
    {
        /// <summary>
        /// 向指定路径文件追加一行
        /// </summary>
        public static void Append(string path, string content)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                File.AppendAllText(path, content + Environment.NewLine);
            }
            catch (Exception e)
            {
                Log.Error($"[FileLogger] 写入失败: {path} | {e.Message}");
            }
        }

        /// <summary>
        /// 按目录 + 端口号拼文件名
        /// </summary>
        public static void AppendForPort(string dir, int port, string content)
            => Append(Path.Combine(dir, $"{port}.log"), content);
    }
}
