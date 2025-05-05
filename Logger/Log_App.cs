using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Logger
{
    public class Log_App
    {
        static string FolderPath = Path.Combine(Environment.CurrentDirectory, "Logger");
        public static bool Enable = false;
        public static void WriteLine(string msg)
        {
            if (!Enable) return;

            string FileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string FilePath = Path.Combine(FolderPath, FileName);

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            string Time_txt = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            string output_txt = $"{Time_txt},{msg}";

            AppendTextToFile(FilePath, output_txt);
        }

        private static void AppendTextToFile(string filePath, string text)
        {
            // 使用StreamWriter以追加模式写入文本到文件
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                // 写入文本
                writer.WriteLine(text);
                writer.Flush();
                writer.Close();
            }

            //Console
            Console.WriteLine(text);
        }
    }
}
