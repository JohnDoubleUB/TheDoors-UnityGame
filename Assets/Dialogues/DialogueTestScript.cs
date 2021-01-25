using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTestScript : MonoBehaviour
{

    private string testFile;
    // Start is called before the first frame update
    void Start()
    {
        testFile = Application.dataPath + "/Dialogues" + "/DialogueTreeTest1.xml";
        DialogueLoader.LoadDialogueFile(testFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
