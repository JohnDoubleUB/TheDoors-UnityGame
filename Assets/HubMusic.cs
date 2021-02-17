using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("Hub_World_Music", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
