using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : FlagManager
{
    public static GameManager current;

    //Platformer related
    private Player player;
    
    [HideInInspector]
    public VerticalPlatform verticalPlatform;

    public List<Door> doors; //make private?
    public List<SaveOptionObject> saveOptionObjects;

    //The action queue is used to store actions for when they can happen
    public List<string> actionQueue = new List<string>(); //TODO: Implement what happens to these actions!

    //private string saveName = "SaveSlot";

    private int selectedSavefile = 0;

    private List<DoorName> CurrentlyDisabledDoors {
        get
        {
            return doors.Where(d => !d.isLit).Select(d => d.doorName).ToList();
        }
    }

    public Player Player {get { return player; }}

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;
        FindKeyComponents();
        LoadSessionData();
        LoadLevelManager();
        //We can add flags here now for testing
        //Debug.Log("(SceneManager) Current Level build index!: " + SceneManager.GetActiveScene().buildIndex + ", and level name: " + SceneManager.GetActiveScene().name);
        //Debug.Log("(SaveSystem.SessionSaveData) Session current Level build index!: " + SaveSystem.SessionSaveData.Level + ", and level name: " + SaveSystem.SessionSaveData.LevelName);
    }

    private void LoadLevelManager()
    {
        switch (SceneManager.GetActiveScene().name) 
        {
            case "Loveworld":
                gameObject.AddComponent<LoveLevelManager>();
                break;
        }
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
            SaveData savedData = SaveSystem.LoadGame(selectedSavefile);


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
        //Tag things, that saves having to do horrible comparisons!
        //Get all the doors!
        GameObject[] sceneDoorGameObjects = GameObject.FindGameObjectsWithTag("WorldDoor");
        if (sceneDoorGameObjects != null && sceneDoorGameObjects.Any()) 
        {
            List<Door> doorComponents = sceneDoorGameObjects.Select(x => x.GetComponent<Door>()).Where(x => x != null).ToList(); // Added null check
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
        return GenerateSaveData(null, -1);
    }

    private SaveData GenerateSaveData(string targetSceneName, int targetSceneBuildIndex) //Responsible for generating our save data!
    {
        Scene activeScene = SceneManager.GetActiveScene(); //Get our current scene for convenience

        List<LevelSaveData> levelData;

        //This should allow us to persist player location in addition to other things in the save data! (hopefully)
        //Check whether we currently have save data in our session
        if (SaveSystem.SessionSaveData != null)
        {
            levelData = SaveSystem.SessionSaveData.LevelData.Select(x => (LevelSaveData)x).Where(x => x.LevelName != activeScene.name).ToList(); //Get all the levelSaveData we have currently in our session except any for our current session
        }
        else 
        {
            levelData = new List<LevelSaveData>(); //If we happen to not have any existing session data then we make this list from scratch
        }

        levelData.Add(new LevelSaveData(activeScene.name, activeScene.buildIndex, player.transform.position)); //Add our current levels levelSaveData

        //If we are not on the hubworld we aren't gonna have the doors and so currentlyDisabledDoors is going to be confused, so we need to leave that as is in our save

        List<DoorName> disabledDoors = doors != null && doors.Any() ?
            CurrentlyDisabledDoors : 
            SaveSystem.SessionSaveData != null ?
            SaveSystem.SessionSaveData.CompletedDoors.Select(x => (DoorName)x).ToList() : 
            new List<DoorName>(); //If we have session save data currently 

        //Generate our new save!
        return new SaveData(
            selectedSavefile, 
            targetSceneName != null ? targetSceneName : activeScene.name, 
            targetSceneBuildIndex != -1 ? targetSceneBuildIndex : activeScene.buildIndex, 
            player.transform.position, 
            disabledDoors, 
            Flags, 
            actionQueue, 
            levelData
            );
    }

    private void UpdateSessionData() 
    {
        UpdateSessionData(GenerateSaveData());
    }

    private void UpdateSessionData_WithTargetScene(string targetSceneName, int targetSceneBuildIndex) 
    {
        UpdateSessionData(GenerateSaveData(targetSceneName, targetSceneBuildIndex));
    }

    protected override void UpdateSessionFlags(string[] updatedFlags)
    {
        if (SaveSystem.SessionSaveData != null)
        {
            SaveSystem.SessionSaveData.Flags = updatedFlags;
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
        //Get the current level
        //Find associated save data
        LevelSaveData currentLevelSaveData = savedData.LevelData.FirstOrDefault(x => x.LevelName == savedData.LevelName);

        //If we have save data for the player on this level
        if (currentLevelSaveData != null)
        {
            //Player position
            player.transform.position = currentLevelSaveData.PlayerPosition;

            //Move camera
            if (Camera.main) Camera.main.transform.position = new Vector3(currentLevelSaveData.PlayerPosition.x, currentLevelSaveData.PlayerPosition.y, Camera.main.transform.position.z);
        }

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
                so.SetContent(SaveSystem.GetSaveLastModifiedDate(so.saveNumber));
            }
        }
    }

    protected override void QueueActions(params string[] actions)
    {
        if(actions != null) actionQueue.AddRange(actions);
        Debug.Log("actions added!" + string.Join(", ", actionQueue));
    }

    public void ChangeLevel(int buildIndex)
    {
        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(buildIndex);

        if (sceneToLoad != null) 
        {
            UpdateSessionData_WithTargetScene(sceneToLoad.name, buildIndex);
            SceneManager.LoadScene(buildIndex);
        }
    }

    public void ChangeLevel(string levelName) 
    {
        Scene sceneToLoad = SceneManager.GetSceneByName(levelName);
        
        if (sceneToLoad != null) 
        {
            UpdateSessionData_WithTargetScene(levelName, sceneToLoad.buildIndex);
            SceneManager.LoadScene(levelName);
        }
    }
}
