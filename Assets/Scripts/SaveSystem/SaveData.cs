using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int SaveNumber;
    public string LevelName;
    public int Level;
    public Vector3 PlayerPosition;
    public List<DoorName> CompletedDoors;
    public List<string> Flags;
    public List<string> ActionQueue;
    public List<LevelSaveData> LevelData;

    public SaveData(int saveNumber, string levelName, int level, Vector3 playerPosition, List<DoorName> completedDoors, List<string> flags, List<string> actionQueue, List<LevelSaveData> levelData) 
    {
        SaveNumber = saveNumber;
        LevelName = levelName;
        Level = level;
        PlayerPosition = playerPosition;
        CompletedDoors = completedDoors;
        Flags = flags;
        ActionQueue = actionQueue;
        LevelData = levelData;
    }
}

public class LevelSaveData 
{
    public string LevelName;
    public int Level;
    public Vector3 PlayerPosition;

    public LevelSaveData(string levelName, int level, Vector3 playerPosition) 
    {
        LevelName = levelName;
        Level = level;
        PlayerPosition = playerPosition;
    }
}
