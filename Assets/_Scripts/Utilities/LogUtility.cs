using UnityEngine;
using System.IO;

namespace Game
{
    using SaveData;

    public static class LogUtility
    {
        private static SettingsData currentSettingsData => GameData.Instance.CurrentSettingsData;
        private static StreamWriter writer;

        public static void StartLogging(string logName = "log")
        {
            if (writer != null)
                return;
            if (!currentSettingsData.EnableLogs)
                return;

            string logsPath = $"{Application.persistentDataPath}/Logs/";
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            string filePath = $"{logsPath}{logName}_{System.DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt";

            writer = new StreamWriter(filePath, true);
            Application.logMessageReceived += HandleLog;
            WriteLog($"Start logging..");
        }
        public static void StopLogging()
        {
            if (writer == null)
                return;

            WriteLog($"End logging..");
            Application.logMessageReceived -= HandleLog;
            writer.Dispose();
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                writer.WriteLine($"\n{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | {logString} ({type})");
                writer.WriteLine($"StackTrace: {stackTrace}");
            }
            else
            {
                writer.WriteLine($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | {logString} ({type})");
            }
        }

        public static void WriteLog(string logData)
        {
            if (writer == null)
                return;

            writer.WriteLine($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | {logData}");
        }
    }
}
