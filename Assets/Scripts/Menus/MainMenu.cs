using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string saveName = "SaveSlot";
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Continue() 
    {
        SaveData mostRecentSave = SaveSystem.LoadMostRecentSaveGame();
        if (mostRecentSave != null) SaveSystem.SessionSaveData = mostRecentSave;
        StartGame();
    }
}
