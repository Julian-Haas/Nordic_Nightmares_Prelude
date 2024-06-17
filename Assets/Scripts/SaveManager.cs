using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public NaddiValueStorage _storage;

    public void SafeData() 
    {
        string json = JsonUtility.ToJson(_storage);
        File.WriteAllText(Application.persistentDataPath + "/naddiValues.json", json); 
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/naddiValues.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("JSON Inhalt: " + json); // Ausgabe des JSON-Inhalts
            try
            {
                JsonUtility.FromJsonOverwrite(json, _storage);
                Debug.Log("Succesfully loaded Naddi Value storage!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error deserializing JSON: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("There is no save file: " + path);
        }
    }
}
