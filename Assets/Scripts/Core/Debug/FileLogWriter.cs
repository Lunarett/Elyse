using System;
using System.IO;
using UnityEngine;

public static class FileLogWriter
{
    private static string logFilePath = Application.persistentDataPath + "/game_logs.txt";

    public static void Log(string message, LogType logType = LogType.Info)
    {
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {logType.ToString()} - {message}";
        Debug.Log(logMessage); // Also print to the Unity console if needed.

        try
        {
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write to log file: " + ex.Message);
        }
    }

    public enum LogType
    {
        Info,
        Warning,
        Error
    }
}