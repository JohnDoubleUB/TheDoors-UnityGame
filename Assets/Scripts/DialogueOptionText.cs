using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DialogueOptionText : MonoBehaviour
{
    public int Id 
    {
        get { return id; }
    }

    public string DialogueText 
    {
        get { return text.text; }
    }

    private Text text;
    private RectTransform rectTransform;
    private int id;


    private void Awake()
    {
        text = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitializeOption(int id, string dialogueText)
    {
        string test = id == 1 ? " a bunch of words to try and break this dialogue stuff yeah yeah yeah blah ooof because reasons ops" : "";
        this.id = id;
        text.text = (this.id+1) + ". " + dialogueText + test;
        
        Canvas.ForceUpdateCanvases(); //Prefered height doesn't get updated until canvas updates, which isn't as regular (that is insanely annoying)
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, text.preferredHeight);
    }

    public void SelectOption() 
    {
        if (DialogueManager.current != null) DialogueManager.current.SelectOption(id);
    }
}
