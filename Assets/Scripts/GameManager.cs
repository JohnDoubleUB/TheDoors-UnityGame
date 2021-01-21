using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current;
    public PlatformerPlayer player;

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;
    }

    private void Update()
    {
        DetectPauseGame();      
    }

    private void DetectPauseGame() 
    {
        //Set UI and stuff
        if (Input.GetButtonDown("Cancel") && UIManager.current != null)
        {
            UIManager.current.ToggleContexts(UIContextType.PauseMenu);
            UIManager.current.SetContextsActive(true, UIContextType.PauseMain);
            UIManager.current.SetContextsActive(false, UIContextType.LoadMenu, UIContextType.SaveMenu);
        }
    }

    public void InitiateSave() 
    {
        if (player != null) 
        {
            Debug.Log("Initiating Save");
            SaveData newSave = new SaveData(SceneManager.GetActiveScene().buildIndex, "TestSave1", player.transform.position);
            SaveSystem.SaveGame(newSave);
        }
    }

    public void InitiateLoad()
    {
        if (player != null)
        {
            Debug.Log("Initiating Load");
            SaveData savedData = SaveSystem.LoadGame();
            Vector3 playerPosition = new Vector3(savedData.Position[0], savedData.Position[1], savedData.Position[2]);

            player.transform.position = playerPosition;
        }
    }
}
