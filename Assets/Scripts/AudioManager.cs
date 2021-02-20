using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;

    private Dictionary<string, string> levelStateSoundEffect = new Dictionary<string, string>();

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
    }

    public uint PlaySoundEvent(string eventName, GameObject playSoundOn) 
    {
        uint result = AkSoundEngine.PostEvent(eventName, playSoundOn);
        return result;
    }

    //public void StopSoundEventImmediate(uint event, GameObject soundPlayingOn) 
    //{
    //    AkSoundEngine.StopPlayingID()
    //}

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
                AkSoundEngine.SetState("Movement", "Hub_World");
                break;
            
            case "Loveworld":
                AkSoundEngine.StopAll();
                AkSoundEngine.SetState("Movement", "Love");
                break;
            
            default:
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("Test_Event", gameObject);
                AkSoundEngine.SetState("Movement", "Love");
                break;
        }
    }
}
