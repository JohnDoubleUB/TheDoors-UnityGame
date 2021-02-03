﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager current;

    private DialogueObject loadedDialogueObject;
    private DialogueTree loadedDialogueTree;
    private Dialogue loadedDialogue;

    public Text speakerText;
    public GameObject dialogueBox;
    public GameObject dialogueOptionPrefab;

    private List<DialogueOptionText> dialogueOptions = new List<DialogueOptionText>();
    
    private int speakerDialogueNo = 0; // Used to allow separate lines to be displayed separately instead of together joined.

    private void Awake()
    {
        if (current == null) current = this;

        //This is for testing purposes


    }

    // Start is called before the first frame update
    void Start()
    {
        //This will need to be changed later
        string[] testDialogueNames = { "intro", "who-did-this-sven" };
        
        LoadDialogueTree("SvenAndPlayer", testDialogueNames[0]);
        LoadUIDialogue(loadedDialogueTree.Dialogues["1"]);
        if (DialogueLoader.DialogueObjects != null) 
        {
            Debug.Log("Dialogue object names: " + string.Join(", ", DialogueLoader.DialogueObjects.Select(x => x.Name)));
        }

        Debug.Log(string.Join(", ", loadedDialogueObject.Speakers));
        //DialogueLoader.LoadAllDialogueObjects();
    }


    public void LoadDialogueTree(string dialogueObjectName, string treeName)
    {
        loadedDialogueObject = DialogueLoader.DialogueObjects.First(x => x.Name == dialogueObjectName);
        loadedDialogueTree = loadedDialogueObject.DialogueTrees.First(x => x.Name == treeName);
    }

    public void LoadUIDialogue(Dialogue dialogue)
    {
        //Clear previous dialogue
        ClearUIDialogue();

        //Load speaker text and speaker name
        if (speakerText != null && dialogue.SpeakerDialogues != null && dialogue.SpeakerDialogues.Any())
        {
            SpeakerDialogue speaker = dialogue.SpeakerDialogues[speakerDialogueNo];
            speakerText.text = loadedDialogueObject.GetSpeakerWithCapital(speaker.SpeakerId) + ": " + speaker.Text;
        }

        //Generate Options
        if (speakerDialogueNo != (dialogue.SpeakerDialogues.Length - 1)) 
        {
            CreateDialogueOptionText(0, "Continue.");
        }
        else if (dialogue.EndsConversation)
        {
            CreateDialogueOptionText(0, "End Conversation.");
        }
        else if (dialogueBox != null && dialogueOptionPrefab && dialogue.DialogueOptions != null && dialogue.DialogueOptions.Any())
        {
            for (int i = 0; i < dialogue.DialogueOptions.Length; i++) CreateDialogueOptionText(i, dialogue.DialogueOptions[i].OptionText);
        }

        loadedDialogue = dialogue;
    }

    public void SelectOption(int option)
    {
        //If this is the end of a conversation then whatever was clicked ends the conversation;
        if (loadedDialogue.SpeakerDialogues != null && 
            loadedDialogue.SpeakerDialogues.Any() && 
            speakerDialogueNo != loadedDialogue.SpeakerDialogues.Length - 1) 
        {
            speakerDialogueNo++;
            LoadUIDialogue(loadedDialogue);
        }
        else if (loadedDialogue.EndsConversation)
        {
            speakerDialogueNo = 0;
            ClearUIDialogue();
        }
        else
        {
            speakerDialogueNo = 0;
            LoadSelectedOption(loadedDialogue.DialogueOptions[option]);
        }
    }

    private void CreateDialogueOptionText(int index, string optionText) 
    {
        DialogueOptionText newDialogueOption = Instantiate(dialogueOptionPrefab, dialogueBox.transform).GetComponent<DialogueOptionText>();
        newDialogueOption.InitializeOption(index, optionText);
        dialogueOptions.Add(newDialogueOption);
    }

    private void ClearUIDialogue()
    {
        if (speakerText != null) speakerText.text = "";

        //clear any existing dialogue options
        for (int j = 0; j < dialogueOptions.Count; j++)
        {
            DestroyImmediate(dialogueOptions[j].gameObject);
        }
        dialogueOptions.Clear();
        loadedDialogue = null;
    }

    private void LoadSelectedOption(DialogueOption option)
    {
        //End conversation if this is the end of the dialogue
        if (option.EndsConversation)
        {
            ClearUIDialogue();
        }
        else if (loadedDialogueTree.Dialogues.TryGetValue(option.DialogueID, out Dialogue newDialogue)) //Otherwise if we can get the new dialogue then do that
        {
            //Check whether this new Dialogue ends the conversation because if it doesn't and we are given no dialogue options then we use the previous dialogues options
            bool retainPreviousOptions = !newDialogue.EndsConversation && (newDialogue.DialogueOptions == null || !newDialogue.DialogueOptions.Any());

            LoadUIDialogue(
                retainPreviousOptions ?
                new Dialogue(newDialogue.Id, newDialogue.SpeakerDialogues, newDialogue.EndsConversation, loadedDialogue.DialogueOptions) :
                newDialogue
            );
        }
    }
}
