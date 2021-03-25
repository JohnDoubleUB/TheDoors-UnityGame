using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;

    private Dictionary<string, string> levelStateSoundEffect = new Dictionary<string, string>();

    private AK.Wwise.Event TestEvent;

    private void Awake()
    {
        if (current == null) current = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        SetWorldState();
    }

    // Update is called once per frame
    void Update()
    {
        //PlaySoundEvent("Jump", gameObject);
        //AkSoundEngine.GetPlayingIDsFromGameObject(gameObject,);
    }

    public uint PlaySoundEvent(string eventName, GameObject playSoundOn) 
    {
        uint result = AkSoundEngine.PostEvent(eventName, playSoundOn);
        return result;
    }

    public void PlayMusicTrack() //Assumed to be played on itself
    {

    }

    private void SetWorldState() 
    {
        switch (SceneManager.GetActiveScene().name) 
        {
            case "Hubworld":
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("Hub_World_Music", gameObject);
                //Check if event is playing using GetPlayingIDsFromGameObject https://www.audiokinetic.com/qa/5379/check-if-a-event-is-playing . Or is there a way to loop some other way?
                AkSoundEngine.SetState("Movement", "Hub_World");
                break;
            
            case "Loveworld":
                AkSoundEngine.StopAll();
                AkSoundEngine.SetState("Movement", "Love");
                break;
            
            default:
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("Test_Event2", gameObject, (uint)AkCallbackType.AK_MusicSyncBar, SyncBarCallback, null);
                AkSoundEngine.SetState("Movement", "None");
                break;
        }
    }

    private void SyncBarCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        Debug.Log("This is a callback from Wwise!");
    }
}
