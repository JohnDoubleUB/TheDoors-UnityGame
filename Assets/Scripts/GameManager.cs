using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

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
}
