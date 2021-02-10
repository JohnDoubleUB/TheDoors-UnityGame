using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : FlagManager
{
    public static GameManager current;

    //Platformer related
    public PlatformerPlayer player;
    public VerticalPlatform verticalPlatform;

    public List<Door> doors; //make private?
    public List<SaveOptionObject> saveOptionObjects;

    //The action queue is used to store actions for when they can happen
    public List<string> actionQueue = new List<string>(); //TODO: Implement what happens to these actions!

    private string saveName = "SaveSlot";

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

        /* I think we should be having the game manager find the player, doors and anything it needs to know about rather than having them find it
         * Mainly because this means that all the other objects need to use a lifecyclehook that logically occurs AFTER Awake which means we end up with
         * and because creating a savegame requires us knowing about the player, doors etc these then have to logically occur AFTER the doors lifecyclehook 
         * all of a sudden we are using Awake, Start, and also the first call of Update, this is not ideal.
         */

        FindKeyComponents(); //New thing!
        //Testing
        //AddFlag("playthrough2");
    }
    private void Update()
    {
        //TODO: Maybe look into this
        if (firstUpdate) //I think this is here because the doors use awake to notify the game manager and that's a problem? or its the awakeness current thing? idk
        {
            if (SaveSystem.SessionSaveData != null) //Ensures save data is loaded if it needs to be
            {
                LoadGameData(SaveSystem.SessionSaveData);
            }
            else 
            {
                UpdateSessionData(); //This way we always have a current savedata
            }

            if (DebugUIText.current != null) DebugUIText.current.SetText("Flags: " + string.Join(", ", SaveSystem.SessionSaveData.Flags));
            firstUpdate = false;
        }
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
            SaveSystem.SaveGame(GenerateSaveData());
            RefreshSaveSlotData();
        }
    }

    public void InitiateLoad()
    {
        if (player != null && selectedSavefile != 0)
        {
            SaveData savedData = SaveSystem.LoadGame(saveName + selectedSavefile);


            if (savedData != null)
            {
                UpdateSessionData(savedData);
                SceneManager.LoadScene(savedData.Level);
            }
        }
    }

    private void FindKeyComponents() //Find things like doors and the player etc,
    {
        Debug.Log("Find key components!");
        //Tag things, that saves having to do horrible comparisons!
        GameObject[] sceneDoorGameObjects = GameObject.FindGameObjectsWithTag("WorldDoor");
        if (sceneDoorGameObjects != null && sceneDoorGameObjects.Any()) 
        {
            List<Door> doorComponents = sceneDoorGameObjects.Select(x => x.GetComponent<Door>()).ToList();
            if (doorComponents.Any()) doors = doorComponents;
        }
        

    }
    private SaveData GenerateSaveData() 
    {
        return new SaveData(saveName + selectedSavefile, SceneManager.GetActiveScene().buildIndex, player.transform.position, CurrentlyDisabledDoors, Flags, actionQueue);
    }

    private void UpdateSessionData() 
    {
        UpdateSessionData(GenerateSaveData());
    }

    protected override void UpdateSessionFlags(string[] updatedFlags)
    {
        if (SaveSystem.SessionSaveData != null)
        {
            SaveSystem.SessionSaveData.Flags = updatedFlags;
            Debug.Log("session flags: " + string.Join(", ", SaveSystem.SessionSaveData.Flags));
            if (DebugUIText.current != null) DebugUIText.current.SetText("Flags: " + string.Join(", ", SaveSystem.SessionSaveData.Flags));
        }
    }

    private void UpdateSessionData(SaveData saveData)
    {
        SaveSystem.SessionSaveData = saveData;
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
        
        //Flags
        LoadFlags(savedData.Flags);

        //Queued Actions
        actionQueue = savedData.ActionQueue;
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

    protected override void QueueActions(params string[] actions)
    {
        if(actions != null) actionQueue.AddRange(actions);
        Debug.Log("actions added!" + string.Join(", ", actionQueue));
    }
}
