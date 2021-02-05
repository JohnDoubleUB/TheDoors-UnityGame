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
    public List<string> flags = new List<string>();

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

        //Add flags here for testing
        AddFlag("playthrough2");
        //AddFlag("test-isnt-new");
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

    private SaveData GenerateSaveData() 
    {
        return new SaveData(saveName + selectedSavefile, SceneManager.GetActiveScene().buildIndex, player.transform.position, CurrentlyDisabledDoors, flags);
    }

    private void UpdateSessionData() 
    {
        UpdateSessionData(GenerateSaveData());
    }

    private void UpdateSessionFlags() 
    {
        if (SaveSystem.SessionSaveData != null)
        {
            SaveSystem.SessionSaveData.Flags = flags.ToArray();
            Debug.Log("session flags: " + string.Join(", ", SaveSystem.SessionSaveData.Flags));
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
        LoadFlags(savedData.Flags);
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

    //Flag related functions

    private void LoadFlags(List<string> activeFlags)
    {
        flags = activeFlags;
    }

    public void AddFlag(string flag) 
    {
        if (!flags.Contains(flag)) flags.Add(flag);

        UpdateSessionFlags();
    }

    public void AddFlags(string[] flags) 
    {
        foreach (string flag in flags)
        {
            if (!this.flags.Contains(flag)) this.flags.Add(flag);
        }

        UpdateSessionFlags();
    }

    public void RemoveFlag(string flag) 
    {
        if (flags.Contains(flag)) flags.Remove(flag);

        UpdateSessionFlags();
    }

    public void RemoveFlags(string[] flags) 
    {
        foreach (string flag in flags) 
            if (this.flags.Contains(flag)) this.flags.Remove(flag);

        UpdateSessionFlags();
    }

    public void ToggleFlag(string flag) 
    {
        if (flags.Contains(flag)) flags.Remove(flag);
        else flags.Add(flag);

        UpdateSessionFlags();
    }

    public bool HasFlag(string flag) 
    {
        return flags.Contains(flag);
    }

    public bool[] HasFlags(string[] flags) 
    {
        return flags.Select(x => HasFlag(x)).ToArray();
    }

    public bool HasAllFlags(string[] flags) 
    {
        foreach (string flag in flags) 
            if (!HasFlag(flag)) return false;

        return true;
    }

    public bool HasAnyFlags(string[] flags) 
    {
        foreach (string flag in flags)
            if (HasFlag(flag)) return true;

        return false;
    }
}
