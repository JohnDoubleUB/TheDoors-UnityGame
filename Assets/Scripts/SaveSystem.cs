using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//SaveSystem is static so it can only be accessed directly and not instantiated
public static class SaveSystem
{

    public static void SaveGame(SaveData saveData) 
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedatatest.doorsave";

        //File.Exists ensures that we replace the file if it already exists and don't encounter errors
        FileStream stream = new FileStream(path, File.Exists(path) ? FileMode.Create : FileMode.CreateNew);


        formatter.Serialize(stream, saveData);
        stream.Close();

        Debug.Log("File saved: " + path);
    }

    public static SaveData LoadGame() 
    {
        string path = Application.persistentDataPath + "/savedatatest.doorsave";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData savedData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            return savedData;
        }
        else 
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }


}
