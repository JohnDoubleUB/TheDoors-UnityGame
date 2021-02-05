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
        string[] testDialogueNames = { "intro", "after-first-door", "after-a-few-doors2", "testdialogue" };


        LoadDialogueTree("SvenAndPlayer", testDialogueNames[3]);
        LoadUIDialogue(loadedDialogueTree.Dialogues["1"]);

        Debug.Log("All flags: " + string.Join(", ", DialogueLoader.AllAddedFlags));
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
            //filter speaker dialogue
            SpeakerDialogue[] filteredSpeakerDialogue = FilterDialogueByFlagRequirements(dialogue.SpeakerDialogues);
            if (filteredSpeakerDialogue.Length != dialogue.SpeakerDialogues.Length) dialogue = new Dialogue(dialogue, filteredSpeakerDialogue);

            //Load speaker dialogue and a name
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
            for (int i = 0; i < dialogue.DialogueOptions.Length; i++)
            {
                //TODO: this screws up numbering when options are missing, these options need to be filtered the same way that the speaker dialogue is
                if (DialogueMeetsFlagRequirements(dialogue.DialogueOptions[i])) CreateDialogueOptionText(i, dialogue.DialogueOptions[i].OptionText); 
            }
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
            LoadFlags(loadedDialogue);
            speakerDialogueNo = 0;
            ClearUIDialogue();
        }
        else
        {
            LoadFlags(loadedDialogue);
            LoadFlags(loadedDialogue.DialogueOptions[option]);
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
                new Dialogue(newDialogue, loadedDialogue.DialogueOptions) :
                newDialogue
            );
        }
    }

    private void LoadFlags(Dialogue dialogue)
    {
        //Load all flags
        LoadFlags(dialogue.ProgressFlags, dialogue.ActionFlags);
    }

    private void LoadFlags(DialogueOption dialogueOption)
    {
        LoadFlags(dialogueOption.ProgressFlags, dialogueOption.ActionFlags);
    }

    private void LoadFlags(string[] progressFlags, string[] actionFlags)
    {
        if (progressFlags != null)
        {
            GameManager.current.AddFlags(progressFlags);
        }
        if (actionFlags != null)
        {
            GameManager.current.AddFlags(actionFlags);
        }
    }

    private SpeakerDialogue[] FilterDialogueByFlagRequirements(SpeakerDialogue[] speakerDialogue)
    {
        return speakerDialogue.Where(x => DialogueMeetsFlagRequirements(x)).ToArray();
    }

    private bool DialogueMeetsFlagRequirements(DialogueOption dialogueOption) 
    {
        return (dialogueOption.RequiredFlags != null ? GameManager.current.HasAllFlags(dialogueOption.RequiredFlags) : true)
            && (dialogueOption.UnRequiredFlags != null ? !GameManager.current.HasAnyFlags(dialogueOption.UnRequiredFlags) : true);
    }

    private bool DialogueMeetsFlagRequirements(SpeakerDialogue speakerDialogue)
    {
        return (speakerDialogue.RequiredFlags != null ? GameManager.current.HasAllFlags(speakerDialogue.RequiredFlags) : true) 
            && (speakerDialogue.UnRequiredFlags != null ? !GameManager.current.HasAnyFlags(speakerDialogue.UnRequiredFlags) : true);
    }
}
