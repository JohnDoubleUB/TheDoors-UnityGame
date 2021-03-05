using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager current;

    private int currentHealth;

    private List<KeyValuePair<UIContextType, UIContext>> uiContexts = new List<KeyValuePair<UIContextType, UIContext>>();
    private Dictionary<UIContextType, bool> activeContextElements = new Dictionary<UIContextType, bool>();

    private bool activeContextsInitiated;
    private UIContextType[] InitiallyEnabledContexts = { UIContextType.HealthDisplay }; //This is mostly for testing

    private UIState uiState = UIState.None;
    public UIState UIState 
    {
        get 
        {
            return uiState;
        }
    }


    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;
        InitiateActiveElementStatuses();
    }

    private void Start()
    {
        UpdateActiveContexts(); //Doing this ensures all the things in the ui can do first time loading properly
    }

    private void InitiateActiveElementStatuses()
    {
        //Add all the types to the list
        foreach (UIContextType context in (UIContextType[])Enum.GetValues(typeof(UIContextType))) 
        {
            if (InitiallyEnabledContexts == null) activeContextElements.Add(context, false);
            else activeContextElements.Add(context, InitiallyEnabledContexts.Contains(context) ? true : false);
        }
    }

    public void AssignObjectContext(UIContext uiContext) 
    {
        uiContexts.Add(new KeyValuePair<UIContextType, UIContext>(uiContext.type, uiContext));
        if (activeContextsInitiated) uiContext.gameObject.SetActive(activeContextElements[uiContext.type]);
    }

    public void SetContextsActive(bool active, params UIContextType[] uiContexts) 
    {
        //Update all required context elements
        foreach (UIContextType context in uiContexts) activeContextElements[context] = active;

        //Update all required objects
        foreach (GameObject gObj in this.uiContexts
            .Where(x => uiContexts.Contains(x.Value.type))
            .Select(x => x.Value.gameObject)) 
        {
            gObj.SetActive(active);
        }

        UpdateUIState();
    }

    public void ToggleContexts(params UIContextType[] uiContexts) 
    {
        //Toggle all required context elements
        foreach (UIContextType context in uiContexts) activeContextElements[context] = !activeContextElements[context];

        //Update all required objects
        foreach (UIContextObject contextObj in this.uiContexts
            .Where(x => uiContexts.Contains(x.Value.type))
            .Select(x => x.Value))
        {
            contextObj.gameObject.SetActive(activeContextElements[contextObj.type]);
        }

        UpdateUIState();
    }


    private void UpdateActiveContexts() 
    {
        foreach (KeyValuePair<UIContextType, UIContext> uiContextObj in uiContexts) 
        {
            uiContextObj.Value.gameObject.SetActive(activeContextElements[uiContextObj.Value.type]);
        }

        activeContextsInitiated = true;

        UpdateUIState();
    }

    private void UpdateUIState() 
    {
        //Debug.Log("state change");
        if (activeContextElements[UIContextType.GameOverMenu]) 
        {
            uiState = UIState.GameOver;
        }
        else if (activeContextElements[UIContextType.PauseMenu]) 
        {
            uiState = UIState.Pause;
        }
        else if (activeContextElements[UIContextType.Dialogue]) 
        {
            uiState = UIState.Dialogue;
        }
        else
        {
            uiState = UIState.None;
        }
    }

    public void UpdatePlayerHealth(int playerCurrentHealth) 
    {
        currentHealth = playerCurrentHealth;
        foreach (KeyValuePair<UIContextType, UIContext> uiContext in uiContexts) 
        {
            if (uiContext.Key == UIContextType.HealthDisplay)
            {
                ((UIHealthContext)uiContext.Value).SetHealth(playerCurrentHealth); 
            }
        }
    }
}

public enum UIContextType 
{
    PauseMenu,
    PauseMain,
    SaveMenu,
    LoadMenu,
    SaveSelection,
    Dialogue,
    HealthDisplay,
    GameOverMenu
}

public enum UIState 
{
    None,
    Pause,
    Dialogue,
    GameOver
}