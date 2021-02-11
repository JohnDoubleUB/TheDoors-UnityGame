using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : FlagManager
{
    public static GameManager current;

    //Platformer related
    private PlatformerPlayer player;
    public VerticalPlatform verticalPlatform;

    public List<Door> doors; //make private?
    public List<SaveOptionObject> saveOptionObjects;

    //The action queue is used to store actions for when they can happen
    public List<string> actionQueue = new List<string>(); //TODO: Implement what happens to these actions!

    private string saveName = "SaveSlot";

    private int selectedSavefile = 0;

    private List<DoorName> CurrentlyDisabledDoors {
        get
        {
            return doors.Where(d => !d.isLit).Select(d => d.doorName).ToList();
        }
    }

    public PlatformerPlayer Player {get { return player; }}

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;
        FindKeyComponents();
        LoadSessionData();

        Debug.Log("Level build index!: " + SceneManager.GetActiveScene().buildIndex);
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

    private void LoadSessionData() 
    {
        if (SaveSystem.SessionSaveData != null) //Ensures save data is loaded if it needs to be
        {
            LoadGameData(SaveSystem.SessionSaveData);
        }
        else
        {
            UpdateSessionData(); //This way we always have a current savedata
        }

        //Debug text!
        if (DebugUIText.current != null) DebugUIText.current.SetText("Flags: " + string.Join(", ", SaveSystem.SessionSaveData.Flags));
    }

    private void FindKeyComponents() //Find things like doors and the player etc,
    {
        Debug.Log("Find key components!");
        //Tag things, that saves having to do horrible comparisons!

        //TODO: These will only be needed on the first level!
        //Get all the doors!
        GameObject[] sceneDoorGameObjects = GameObject.FindGameObjectsWithTag("WorldDoor");
        if (sceneDoorGameObjects != null && sceneDoorGameObjects.Any()) 
        {
            List<Door> doorComponents = sceneDoorGameObjects.Select(x => x.GetComponent<Door>()).ToList();
            if (doorComponents.Any()) doors = doorComponents;
        }

        //Get the player!
        GameObject[] scenePlayer = GameObject.FindGameObjectsWithTag("Player");
        if (scenePlayer != null && scenePlayer.Any()) 
        {
            PlatformerPlayer pPlayer = scenePlayer[0].GetComponent<PlatformerPlayer>();
            if (pPlayer != null) player = pPlayer;
        }

        //Get the save slots!
        GameObject[] saveSlots = GameObject.FindGameObjectsWithTag("SaveSlot");
        if (saveSlots != null && saveSlots.Any()) 
        {
            foreach (GameObject saveSlotGO in saveSlots) 
            {
                SaveOptionObject saveOption = saveSlotGO.GetComponent<SaveOptionObject>();
                if (saveOption != null) saveOptionObjects.Add(saveOption);
            }
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
