using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTestScript : MonoBehaviour
{
    public Text testText;
    private string testFile;
    private DialogueManager dManager;
    // Start is called before the first frame update
    void Start()
    {
        testFile = Application.dataPath + "/Dialogues" + "/DialogueTreeTest1.xml";
        DialogueObject dObject = DialogueLoader.LoadDialogueFile(testFile);


       // Debug.Log("All the info is here!");



        //Manage dialogue

        //Debug.Log("There are: " + dObject.DialogueTrees.Length + " dialogues for speaker: " + dObject.Speaker + ".");

        DialogueTree introDialogue = dObject.DialogueTrees.First(x => x.Name == "intro");

        if (introDialogue.Dialogues == null) Debug.Log("its null");

       // Debug.Log(dObject.DialogueTrees[0].Name);

        //foreach (KeyValuePair<string, Dialogue> kvp in introDialogue.Dialogues) 
        //{
        //    Debug.Log(kvp.Key);
        //}


        dManager = DialogueManager.GetManager();

        dManager.CurrentDialogueTree = introDialogue;

        //dManager.CurrentDialogueTree.Dialogues;

        //Debug.Log(dManager.GetDialogue());

        SetTestText(dManager.GetDialogue());
    }

    public void SetTestText(string text) 
    {
        if(testText != null) testText.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1")) SetTestText(dManager.SelectDialogueOption(0));
        if (Input.GetKeyDown("2")) SetTestText(dManager.SelectDialogueOption(1));
        if (Input.GetKeyDown("3")) SetTestText(dManager.SelectDialogueOption(2));
        if (Input.GetKeyDown("4")) SetTestText(dManager.SelectDialogueOption(3));
    }
}
