using System.IO;
using UnityEngine;

public static class DebugFileLogger
{
    private static string filePath = "Assets/06_PersonalSpaces/Nils/LogFiles/";

    public static void Initialize()
    {
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
    }

    public static void Log(string fileName, string message)
    {
        string logFilePath = Path.Combine(filePath, fileName);

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{System.DateTime.Now}: {message}");
        }
        Debug.Log("Logged something"); 
    }
}

