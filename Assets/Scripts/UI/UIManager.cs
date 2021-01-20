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

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;
        InitiateActiveElementStatuses();
    }

    private void InitiateActiveElementStatuses()
    {
        //Add all the types to the list
        foreach (UIContextType context in (UIContextType[])Enum.GetValues(typeof(UIContextType))) 
        {
            activeContextElements.Add(context, false);
        }
    }

    public void AssignObjectContext(UIContextObject uiContextObject) 
    {
        uiContextObjects.Add(new KeyValuePair<UIContextType, UIContextObject>(uiContextObject.type, uiContextObject));
        uiContextObject.gameObject.SetActive(activeContextElements[uiContextObject.type]);
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
    }
}

public enum UIContextType 
{
    PauseMenu,
    PauseMain,
    SaveMenu,
    LoadMenu
}