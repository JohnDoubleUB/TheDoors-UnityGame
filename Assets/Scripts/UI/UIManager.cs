using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager current;

    private List<KeyValuePair<UIContextType, UIContextObject>> uiContextObjects = new List<KeyValuePair<UIContextType, UIContextObject>>();
    private Dictionary<UIContextType, bool> activeContextElements = new Dictionary<UIContextType, bool>();

    private bool activeContextsInitiated;
    private UIContextType[] InitiallyEnabledContexts; //= { UIContextType.Dialogue }; //This is mostly for testing

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

    public void AssignObjectContext(UIContextObject uiContextObject) 
    {
        uiContextObjects.Add(new KeyValuePair<UIContextType, UIContextObject>(uiContextObject.type, uiContextObject));
        if (activeContextsInitiated) uiContextObject.gameObject.SetActive(activeContextElements[uiContextObject.type]);
    }

    public void SetContextsActive(bool active, params UIContextType[] uiContexts) 
    {
        //Update all required context elements
        foreach (UIContextType context in uiContexts) activeContextElements[context] = active;

        //Update all required objects
        foreach (GameObject gObj in uiContextObjects
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
        foreach (UIContextObject contextObj in uiContextObjects
            .Where(x => uiContexts.Contains(x.Value.type))
            .Select(x => x.Value))
        {
            contextObj.gameObject.SetActive(activeContextElements[contextObj.type]);
        }

        UpdateUIState();
    }


    private void UpdateActiveContexts() 
    {
        foreach (KeyValuePair<UIContextType, UIContextObject> uiContextObj in uiContextObjects) 
        {
            uiContextObj.Value.gameObject.SetActive(activeContextElements[uiContextObj.Value.type]);
        }

        activeContextsInitiated = true;

        UpdateUIState();
    }

    private void UpdateUIState() 
    {
        //Debug.Log("state change");
        if (activeContextElements[UIContextType.PauseMenu]) 
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
}

public enum UIContextType 
{
    PauseMenu,
    PauseMain,
    SaveMenu,
    LoadMenu,
    SaveSelection,
    Dialogue
}

public enum UIState 
{
    None,
    Pause,
    Dialogue
}