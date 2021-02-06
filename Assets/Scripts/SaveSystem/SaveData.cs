using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public string SaveName;
    public int Level;
    public Vector3 PlayerPosition;
    public List<DoorName> CompletedDoors;
    public List<string> Flags;
    public List<string> ActionQueue;

    public SaveData(string saveName, int level, Vector3 playerPosition, List<DoorName> completedDoors, List<string> flags, List<string> actionQueue) 
    {
        SaveName = saveName;
        Level = level;
        PlayerPosition = playerPosition;
        CompletedDoors = completedDoors;
        Flags = flags;
        ActionQueue = actionQueue;
    }
}
