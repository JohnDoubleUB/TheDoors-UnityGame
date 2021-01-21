using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will be serialized and used to store savedata

[System.Serializable]
public class SaveData
{
    public int Level;
    public string SaveName;
    public float[] Position;

    public SaveData(int level, string saveName, Vector3 position) 
    {
        Level = level;
        SaveName = saveName;
        
        //Store position in serializable type
        Position = new float[3];
        Position[0] = position.x;
        Position[1] = position.y;
        Position[2] = position.z;
    }
}
