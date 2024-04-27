using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SavedPlayer
{
    // for runtime you work with dictonary for saving with the list since dictonary cna't be saved

    // string is the level key and int is the level value 0 not cleared 1 cleared
    [SerializeField, HideInInspector] private List<(string, int)> _completedLevelsList;
    private Dictionary<string,int> _completedLevels;

    private static string _filePath = Application.persistentDataPath + "/player.sav";

    public SavedPlayer() {

        _completedLevels = new Dictionary<string,int>();
        _completedLevelsList = new List<(string, int)>();
    }

    public static SavedPlayer LoadPlayerSave() {

        SavedPlayer saveData;
        if(File.Exists(_filePath)) {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(_filePath,FileMode.Open);
            string json = (string) formatter.Deserialize(stream);
            stream.Close();

            saveData = JsonUtility.FromJson<SavedPlayer>(json);
            saveData.InitDictonary();
        }
        else {
            saveData = new SavedPlayer();
            saveData.SavePlayerSave();
        }
        return saveData;
    }

    // changes should be activly saved and not automaticaly when value changes
    public void SavePlayerSave() {
        _completedLevelsList.Clear();
        foreach(KeyValuePair<string,int> pair in _completedLevels) {
            _completedLevels.Add(pair.Key,pair.Value);
        }
        string json = JsonUtility.ToJson(this);

        // convert json into binary format to make it unreadable and stop external manipulation of save file
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = File.Create(_filePath);
        formatter.Serialize(stream,json);

        stream.Close();
    }

    private void InitDictonary() {
        foreach((string, int) pair in _completedLevelsList) {
            _completedLevels[pair.Item1] = pair.Item2;
        }
    }

    public int CheckLevelCompletion(string key) {

        if(_completedLevels.TryGetValue(key,out int result)) {
            return result;
        }
        return 0;
    }
    public void AddCompletedLevel(string key,int value) {
        _completedLevels[key] = value;
    }
    public void ResetCompletedLevels() {
        _completedLevels.Clear();
    }
}
