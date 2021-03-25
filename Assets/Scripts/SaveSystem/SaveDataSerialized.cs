using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class will be serialized and used to store savedata

[System.Serializable]
public class SaveDataSerialized
{
    public int SaveNumber;
    public string LevelName;
    public int Level;
    public float[] PlayerPosition;
    public int[] CompletedDoors;
    public string[] Flags;
    public string[] ActionQueue;
    public string[] DialogueTreeFlags;
    public LevelSaveDataSerialized[] LevelData;

    public SaveDataSerialized(string levelName, int level, int saveNumber, Vector3 position, List<DoorName> completedDoors, List<string> flags, List<string> actionQueue, List<string> dialogueTreeFlags, List<LevelSaveData> levelData) 
    {
        Level = level;
        SaveNumber = saveNumber;

        //Store position in serializable type
        PlayerPosition = new float[3];
        PlayerPosition[0] = position.x;
        PlayerPosition[1] = position.y;
        PlayerPosition[2] = position.z;

        CompletedDoors = completedDoors.Distinct().Select(dn => (int)dn).ToArray();
        Flags = flags.ToArray();
        ActionQueue = actionQueue.ToArray();
        DialogueTreeFlags = dialogueTreeFlags.ToArray();
        LevelName = levelName;
        LevelData = levelData.Select(x => (LevelSaveDataSerialized)x).ToArray();
    }


    //For easier type conversion
    public static implicit operator SaveDataSerialized(SaveData saveData)
    {
        return new SaveDataSerialized(
            saveData.LevelName,
            saveData.Level, 
            saveData.SaveNumber, 
            saveData.PlayerPosition, 
            saveData.CompletedDoors,
            saveData.Flags,
            saveData.ActionQueue,
            saveData.DialogueTreeFlags,
            saveData.LevelData
            );
    }

    public static implicit operator SaveData(SaveDataSerialized saveData)
    {
        Vector3 playerPositionVector = saveData.PlayerPosition.Length == 3 ? new Vector3(saveData.PlayerPosition[0], saveData.PlayerPosition[1], saveData.PlayerPosition[2]) : new Vector3();

        return new SaveData(
            saveData.SaveNumber,
            saveData.LevelName,
            saveData.Level,
            playerPositionVector,
            saveData.CompletedDoors.Select(dn => (DoorName)dn).ToList(),
            saveData.Flags.ToList(),
            saveData.ActionQueue.ToList(),
            saveData.DialogueTreeFlags.ToList(),
            saveData.LevelData.Select(x => (LevelSaveData)x).ToList()
            );
    }
}

[System.Serializable]
public class LevelSaveDataSerialized
{
    public string LevelName;
    public int Level;
    public float[] PlayerPosition;

    public LevelSaveDataSerialized(string levelName, int level, Vector3 playerPosition)
    {
        LevelName = levelName;
        Level = level;

        PlayerPosition = new float[3];
        PlayerPosition[0] = playerPosition.x;
        PlayerPosition[1] = playerPosition.y;
        PlayerPosition[2] = playerPosition.z;
    }

    public static implicit operator LevelSaveDataSerialized(LevelSaveData levelSaveData) 
    {
        return new LevelSaveDataSerialized(levelSaveData.LevelName, levelSaveData.Level, levelSaveData.PlayerPosition);
    }

    public static implicit operator LevelSaveData(LevelSaveDataSerialized levelSaveData) 
    {
        Vector3 playerPosition = levelSaveData.PlayerPosition.Length == 3 ? new Vector3(levelSaveData.PlayerPosition[0], levelSaveData.PlayerPosition[1], levelSaveData.PlayerPosition[2]) : new Vector3();
        return new LevelSaveData(levelSaveData.LevelName, levelSaveData.Level, playerPosition);
    }
}