using System;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private static string logFilePath;

    private void Awake()
    {
        // Set a custom log file path
        string customPath = "D:/UnityLogJJ"; // Change this to your preferred location
        
        // Ensure directory exists
        if (!Directory.Exists(customPath))
        {
            Directory.CreateDirectory(customPath);
        }

        // Define full log file path
        logFilePath = Path.Combine(customPath, "GameLog.txt");

        // Start log file
        File.WriteAllText(logFilePath, "Log Started: " + DateTime.Now + "\n");

        // Capture all Unity console logs
        Application.logMessageReceived += HandleUnityLog;
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{DateTime.Now} [{type}] {logString}\n{stackTrace}";
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }

    public static void LogMessage(string message)
    {
        string logEntry = $"{DateTime.Now} - {message}";
        Debug.Log(logEntry);
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
    }
}
