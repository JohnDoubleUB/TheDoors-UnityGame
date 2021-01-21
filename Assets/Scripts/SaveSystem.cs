using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

//SaveSystem is static so it can only be accessed directly and not instantiated
public static class SaveSystem
{
    private static string saveExtension = ".doorsave";

    public static void SaveGame(SaveData saveData) 
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedatatest" + saveExtension;

        //File.Exists ensures that we replace the file if it already exists and don't encounter errors
        FileStream stream = new FileStream(path, File.Exists(path) ? FileMode.Create : FileMode.CreateNew);


        formatter.Serialize(stream, saveData);
        stream.Close();

        Debug.Log("File saved: " + path);
    }

    public static SaveData LoadGame() 
    {
        string path = Application.persistentDataPath + "/savedatatest" + saveExtension;
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

    public static string[] GetSaves() 
    {
        return Directory.GetFiles(Application.persistentDataPath).Where(s => s.EndsWith(saveExtension)).ToArray();
    }


}
