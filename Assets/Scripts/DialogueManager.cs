using System.Collections;
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
    public bool includeDialogueOptionNumbers;

    private List<DialogueOptionText> dialogueOptions = new List<DialogueOptionText>();

    private int speakerDialogueNo = 0; // Used to allow separate lines to be displayed separately instead of together joined.

    private void Awake()
    {
        if (current == null) current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //This will need to be changed later
        string[] testDialogueNames = { "intro", "after-first-door", "after-a-few-doors2", "testdialogue" };


        LoadDialogueTree("SvenAndPlayer", testDialogueNames[3]);
        LoadUIDialogueFrame(loadedDialogueTree.Dialogues["1"]);

        Debug.Log("All flags: " + string.Join(", ", DialogueLoader.AllAddedFlags));
    }


    public void LoadDialogueTree(string dialogueObjectName, string treeName)
    {
        loadedDialogueObject = DialogueLoader.DialogueObjects.First(x => x.Name == dialogueObjectName);
        loadedDialogueTree = loadedDialogueObject.DialogueTrees.First(x => x.Name == treeName);
    }

    public void LoadUIDialogueFrame(Dialogue dialogue)
    {
        //Clear previous dialogue
        ClearUIDialogue();

        //Apply all the flag requirement filters
        Dialogue filteredDialogue = FilterDialogueByFlagRequirements(dialogue);

        //Load speaker text and speaker name
        if (speakerText != null && filteredDialogue.SpeakerDialogues != null && filteredDialogue.SpeakerDialogues.Any())
        {
            //Load speaker dialogue and a name
            SpeakerDialogue speaker = filteredDialogue.SpeakerDialogues[speakerDialogueNo];
            speakerText.text = loadedDialogueObject.GetSpeakerWithCapital(speaker.SpeakerId) + ": " + speaker.Text;
        }

        //Generate Options
        if (speakerDialogueNo != (filteredDialogue.SpeakerDialogues.Length - 1))
        {
            CreateDialogueOptionText(0, "Continue.");
        }
        else if (filteredDialogue.EndsConversation)
        {
            CreateDialogueOptionText(0, "End Conversation.");
        }
        else if (dialogueBox != null && dialogueOptionPrefab && filteredDialogue.DialogueOptions != null && filteredDialogue.DialogueOptions.Any())
        {
            for (int i = 0; i < filteredDialogue.DialogueOptions.Length; i++)
            {
                CreateDialogueOptionText(i, filteredDialogue.DialogueOptions[i].OptionText); 
            }
        }

        loadedDialogue = filteredDialogue;
    }

    public void SelectOption(int option)
    {
        //If this is the end of a conversation then whatever was clicked ends the conversation;
        if (loadedDialogue.SpeakerDialogues != null &&
            loadedDialogue.SpeakerDialogues.Any() &&
            speakerDialogueNo != loadedDialogue.SpeakerDialogues.Length - 1)
        {
            speakerDialogueNo++;
            LoadUIDialogueFrame(loadedDialogue);
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
        newDialogueOption.InitializeOption(index, optionText, includeDialogueOptionNumbers);
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

            LoadUIDialogueFrame(
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

    private DialogueOption[] FilterDialogueByFlagRequirements(DialogueOption[] dialogueOption)
    {
        return dialogueOption.Where(x => DialogueMeetsFlagRequirements(x)).ToArray();
    }

    private Dialogue FilterDialogueByFlagRequirements(Dialogue dialogueToFilter)
    {
        DialogueOption[] filteredDialogueOptions = null;
        SpeakerDialogue[] filteredSpeakerDialogue = null;

        if (dialogueToFilter.DialogueOptions != null && dialogueToFilter.DialogueOptions.Any()) filteredDialogueOptions = FilterDialogueByFlagRequirements(dialogueToFilter.DialogueOptions);
        if (speakerText != null && dialogueToFilter.SpeakerDialogues != null && dialogueToFilter.SpeakerDialogues.Any()) filteredSpeakerDialogue = FilterDialogueByFlagRequirements(dialogueToFilter.SpeakerDialogues);

        bool speakerDialoguesNeedsUpdating = filteredSpeakerDialogue != null && filteredSpeakerDialogue.Length != dialogueToFilter.SpeakerDialogues.Length;
        bool dialogueOptionsNeedUpdating = filteredDialogueOptions != null && filteredDialogueOptions.Length != dialogueToFilter.DialogueOptions.Length;

        if (speakerDialoguesNeedsUpdating && dialogueOptionsNeedUpdating)
        {
            return new Dialogue(dialogueToFilter, filteredSpeakerDialogue, filteredDialogueOptions);
        }
        else if (speakerDialoguesNeedsUpdating)
        {
            return new Dialogue(dialogueToFilter, filteredSpeakerDialogue);
        }
        else if (dialogueOptionsNeedUpdating)
        {
            return new Dialogue(dialogueToFilter, filteredDialogueOptions);
        }

        return dialogueToFilter;
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
