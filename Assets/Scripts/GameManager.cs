using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current;
    public PlatformerPlayer player;
    public List<Door> doors;
    //private List<DoorName> doorNames = new List<DoorName>() { DoorName.Tent };

    private bool firstUpdate = true;

    private List<DoorName> CurrentlyDisabledDoors { 
        get 
        {
            return doors.Where(d => !d.isLit).Select(d => d.doorName).ToList();
        } 
    }

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;
    }

    private void Update()
    {
        if (firstUpdate) 
        {
            if (SaveSystem.CurrentSaveData != null) LoadGameData(SaveSystem.CurrentSaveData); //Ensures save data is loaded if it needs to be

            firstUpdate = false;
        }
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

    public void AddDoor(Door door) 
    {
        doors.Add(door);
    }

    public void InitiateSave() 
    {
        if (player != null) 
        {
            Debug.Log("Initiating Save");
            SaveData newSaveData = new SaveData("TestSave1", SceneManager.GetActiveScene().buildIndex, player.transform.position, CurrentlyDisabledDoors);
            SaveSystem.SaveGame(newSaveData);
        }
    }

    public void InitiateLoad()
    {
        if (player != null)
        {
            Debug.Log("Initiating Load");
            SaveData savedData = SaveSystem.LoadGame();


            LoadGameData(savedData);
            //player.transform.position = savedData.PlayerPosition;

        }
    }

    private void UpdateDoors(bool enabled, List<DoorName> doorsToUpdate) 
    {
        foreach (Door door in doors) 
        {
            door.isLit = doorsToUpdate.Contains(door.doorName) ? enabled : true;
        }
    }

    private void LoadGameData(SaveData savedData)
    {
        //Player position
        player.transform.position = savedData.PlayerPosition;

        //Door states
        UpdateDoors(false, savedData.CompletedDoors);
    }
}
