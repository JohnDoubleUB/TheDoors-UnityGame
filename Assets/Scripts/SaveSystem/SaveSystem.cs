using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

//SaveSystem is static so it can only be accessed directly and not instantiated
public static class SaveSystem
{
    private static string saveExtension = ".doorsave";
    public static SaveDataSerialized SessionSaveData; // Session save data
    public static string currentTextTest;

    /* 
    ------ SAVE SYSTEM NOTES (TODO?) ------
    Save System:
        3 save slots? or just one?

    Save contents:

        Level:
        Player position:
        Checkpoint? (For door levels only)
        Completed doors
        OptionsFlags (This will be things that define a playthrough)

    */

    public static void SaveGame(SaveData saveData) 
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + saveData.SaveName + saveExtension;

        //File.Exists ensures that we replace the file if it already exists and don't encounter errors
        FileStream stream = new FileStream(path, File.Exists(path) ? FileMode.Create : FileMode.CreateNew);


        formatter.Serialize(stream, (SaveDataSerialized)saveData);
        stream.Close();

        Debug.Log("File saved: " + path);
    }

    public static SaveData LoadGame(string saveName)
    {
        string path = Application.persistentDataPath + "/" + saveName + saveExtension;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveDataSerialized savedData = (SaveDataSerialized)formatter.Deserialize(stream);
            stream.Close();

            return (SaveData)savedData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static string GetSaveLastModifiedDate(string saveName) 
    {
        string path = Application.persistentDataPath + "/" + saveName + saveExtension;
        return File.Exists(path) ? File.GetLastAccessTime(path).ToString() : "";
    }


}
