using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager current;
    private DialogueTree loadedDialogue;

    public Text speakerText;
    public GameObject dialogueBox;
    public GameObject dialogueOptionPrefab;

    private List<DialogueOptionText> dialogueOptions = new List<DialogueOptionText>();

    private void Awake()
    {
        if (current == null) current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //This will need to be changed later
        LoadDialogueTree("DialogueTreeTest1", "intro");

        Dialogue d = loadedDialogue.Dialogues["1"];
        LoadDialogue(d);
        //if (loadedDialogue != null) Debug.Log("test log:" + string.Join(",", loadedDialogue.Dialogues["1"].Speaker));
    }


    public void LoadDialogueTree(string dialogueObjectName, string treeName) 
    {
        loadedDialogue = DialogueLoader.LoadDialogueFile(dialogueObjectName).DialogueTrees.First(x => x.Name == treeName);
    }


    public void LoadDialogue(Dialogue dialogue) 
    {        
        if (speakerText != null && dialogue.Speaker.Length > 0)
        {
            speakerText.text = string.Join(",", dialogue.Speaker);
        }
        
        if (dialogueBox != null && dialogueOptionPrefab && dialogue.DialogueOptions.Length > 0) 
        {
            for (int i = 0; i < dialogue.DialogueOptions.Length; i++) 
            {
                DialogueOption dOption = dialogue.DialogueOptions[i];

                DialogueOptionText newDialogueOption = Instantiate(dialogueOptionPrefab, dialogueBox.transform).GetComponent<DialogueOptionText>();
                newDialogueOption.InitializeOption(i, dOption.OptionText);
                dialogueOptions.Add(newDialogueOption);
            }
        }

    }

    public void SelectOption(int option) 
    {
        print("option " + option + " was selected!");
        //Figure out what to do with input from dialogue system thing?
    }
}
