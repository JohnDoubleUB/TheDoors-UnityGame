using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class will be serialized and used to store savedata

[System.Serializable]
public class SaveDataSerialized
{
    public string SaveName;
    public int Level;
    public float[] PlayerPosition;
    public int[] CompletedDoors;

    public SaveDataSerialized(int level, string saveName, Vector3 position, List<DoorName> completedDoors) 
    {
        Level = level;
        SaveName = saveName;

        //Store position in serializable type
        PlayerPosition = new float[3];
        PlayerPosition[0] = position.x;
        PlayerPosition[1] = position.y;
        PlayerPosition[2] = position.z;

        CompletedDoors = completedDoors.Distinct().Select(dn => (int)dn).ToArray();

    }


    //For easier type conversion
    public static implicit operator SaveDataSerialized(SaveData saveData)
    {
        return new SaveDataSerialized(
            saveData.Level, 
            saveData.SaveName, 
            saveData.PlayerPosition, 
            saveData.CompletedDoors
            );
    }

    public static implicit operator SaveData(SaveDataSerialized saveData)
    {
        Vector3 playerPositionVector = saveData.PlayerPosition.Length == 3 ? new Vector3(saveData.PlayerPosition[0], saveData.PlayerPosition[1], saveData.PlayerPosition[2]) : new Vector3();

        return new SaveData(
            saveData.SaveName,
            saveData.Level,
            playerPositionVector,
            saveData.CompletedDoors.Select(dn => (DoorName)dn).ToList()
            );
    }
}
