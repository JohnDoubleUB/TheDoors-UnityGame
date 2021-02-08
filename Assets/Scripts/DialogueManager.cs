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
        //string[] testDialogueNames = { "intro", "after-first-door", "after-a-few-doors2", "testdialogue" };

        //TODO: This loads after the flags are added from loading, oof
        //LoadDialogueTree("SvenAndPlayer", testDialogueNames[0]);

        //Debug.Log("All flags: " + string.Join(", ", DialogueLoader.AllAddedFlags));
    }

    public void LoadDialogueTree(string dialogueObjectName, string treeName)
    {
        DialogueObject newlyLoadedDialogueObject = DialogueLoader.DialogueObjects.First(x => x.Name == dialogueObjectName);
        DialogueTree newlyLoadedDialogueTree = newlyLoadedDialogueObject != null ? newlyLoadedDialogueObject.DialogueTrees.First(x => x.Name == treeName) : null;

        if (newlyLoadedDialogueTree != null)
        {
            loadedDialogueObject = newlyLoadedDialogueObject;
            loadedDialogueTree = newlyLoadedDialogueTree;
            if (UIManager.current != null) UIManager.current.SetContextsActive(true, UIContextType.Dialogue); //Update UI contexts
            LoadUIDialogueFrame(loadedDialogueTree.EntryPoint);
        }
        else 
        {
            Debug.LogError((newlyLoadedDialogueObject != null ? 
                "DialogueTree with Name `" + treeName + "` does not exist." : 
                "DialogueObject with Name `" + dialogueObjectName + "` does not exist.") 
                + " Dialogue was not loaded.");
        }
    }

    public void SelectOption(int option)
    {
        //If this is the end of a conversation then whatever was clicked ends the conversation;
        if (loadedDialogue.SpeakerDialogues != null &&
            loadedDialogue.SpeakerDialogues.Any() &&
            speakerDialogueNo != loadedDialogue.SpeakerDialogues.Length - 1) //Loop until all speaker dialogue has been displayed
        {
            speakerDialogueNo++;
            LoadUIDialogueFrame(loadedDialogue); //TODO: This is a little inefficent maybe? I don't know if I care enough to fix this its not a big issue
        }
        else if (loadedDialogue.EndsConversation) //If the loaded dialogue ends conversation
        {
            //If a dialogue frame is the end of a convesation
            LoadFlags(loadedDialogue);
            speakerDialogueNo = 0;
            EndConversation();
        }
        else
        {
            //This is where we care about dialogue options
            //Load flags for selected dialogue option AND any of the current dialogue frame flags
            LoadFlags(loadedDialogue);
            LoadFlags(loadedDialogue.DialogueOptions[option]);
            speakerDialogueNo = 0;
            LoadSelectedOption(loadedDialogue.DialogueOptions[option]);
        }
    }

    private void LoadUIDialogueFrame(Dialogue dialogue)
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
            EndConversation();
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

    private void EndConversation() 
    {
        //Clear the UI (Dialogue options, speaker text, loaded dialogue etc)
        ClearUIDialogue();

        //Add flag to indicate this dialogue has taken place
        GameManager.current.AddFlag(loadedDialogueTree.Name);

        //Clear all the things
        loadedDialogueObject = null;
        loadedDialogueTree = null;

        //Set the Dialogue context to not be active (i.e. hide the dialogue ui because dialogue has ended)
        if (UIManager.current != null) UIManager.current.SetContextsActive(false, UIContextType.Dialogue);
    }

    //Flag loading code
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
            GameManager.current.AddActionFlags(actionFlags);
        }
    }

    //Flag requirement filtering code
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
}
