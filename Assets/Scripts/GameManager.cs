using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current;
    
    //Platformer related
    public PlatformerPlayer player;
    public VerticalPlatform verticalPlatform;

    public List<Door> doors; //make private?
    public List<SaveOptionObject> saveOptionObjects;
    private string saveName = "SaveSlot";
    //private List<DoorName> doorNames = new List<DoorName>() { DoorName.Tent }; sdasd

    private int selectedSavefile = 0;



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
    }

    public void AddDoor(Door door) 
    {
        doors.Add(door);
    }

    public void AddSaveOptionObject(SaveOptionObject saveOptionObject) 
    {
        saveOptionObjects.Add(saveOptionObject);
    }

    public void SetSelectedSaveOption(int optionNo)
    {
        selectedSavefile = optionNo;

        if (saveOptionObjects.Count > 0) 
        {
            foreach (SaveOptionObject so in saveOptionObjects) 
            {
                so.IsSelected = so.saveNumber == optionNo;
            }
        }
    }

    public void InitiateSave() 
    {
        if (player != null && selectedSavefile != 0) 
        {
            //Debug.Log("Initiating Save");
            SaveData newSaveData = new SaveData(saveName + selectedSavefile, SceneManager.GetActiveScene().buildIndex, player.transform.position, CurrentlyDisabledDoors);
            SaveSystem.SaveGame(newSaveData);
            RefreshSaveSlotData();
        }
    }

    public void InitiateLoad()
    {
        if (player != null && selectedSavefile != 0)
        {
            //Debug.Log("Initiating Load");
            SaveData savedData = SaveSystem.LoadGame(saveName + selectedSavefile);


            if(savedData != null) 
            {
                SaveSystem.CurrentSaveData = savedData;
                SceneManager.LoadScene(savedData.Level);
            }
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

        //Move camera
        if (Camera.main) Camera.main.transform.position = new Vector3(savedData.PlayerPosition.x, savedData.PlayerPosition.y, Camera.main.transform.position.z);

        //Door states
        UpdateDoors(false, savedData.CompletedDoors);
    }

    public void RefreshSaveSlotData()
    {
        if (saveOptionObjects.Count > 0)
        {
            foreach (SaveOptionObject so in saveOptionObjects)
            {
                so.SetContent(SaveSystem.GetSaveLastModifiedDate(saveName + so.saveNumber));
            }
        }
    }
}
