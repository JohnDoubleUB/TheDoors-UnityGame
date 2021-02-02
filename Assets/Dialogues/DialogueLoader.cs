using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;

public static class DialogueLoader
{
    private static string DialoguePath = "/Dialogues/";
    private static string DialogueFileExtension = "xml";
    //Attributes: id, tree-name, dialogue-id

    public static DialogueObject LoadDialogueFile(string fileName)
    {
        //Build path
        string path = Application.dataPath + DialoguePath + fileName +"." + DialogueFileExtension;

        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Find top level xml node!
            XmlNode topLevelNode = doc.DocumentElement.SelectSingleNode("/dialogue-object");

            XmlNodeList xmlNodes = topLevelNode.SelectNodes("dialogue-tree");

            Debug.Log("Node count" + xmlNodes.Count);

            DialogueTree[] dialogueTrees = new List<XmlNode>(xmlNodes.Cast<XmlNode>()).Select(x => BuildDialogueTree(x)).ToArray();


            return new DialogueObject(topLevelNode.Attributes["speaker"].InnerText, dialogueTrees);
        }
        else 
        {
            Debug.Log("failed to open");

            return null;
        }
  
    }

    //TODO: run various data through this to make sure it works properly, and genrally cleanup this mess! I haven't run this yet but I think it mostly should work
    private static DialogueTree BuildDialogueTree(XmlNode dialogueTree) 
    {
        List<Dialogue> dList = new List<Dialogue>();
        //First loop

        XmlNodeList xmlDialogues = dialogueTree.SelectNodes("dialogue");

        foreach (XmlNode xmlDialogue in xmlDialogues) 
        {
            List<XmlNode> xmlDialogueList = new List<XmlNode>(xmlDialogue.ChildNodes.Cast<XmlNode>());
            
            string id = xmlDialogue.Attributes["id"].InnerText;
            
            bool endsConversation = xmlDialogueList.Any(x => x.Name == "speaker-end");

            string[] speaker = xmlDialogueList
                .Where(x => x.Name == "speaker" || x.Name == "speaker-end")
                .Select(x => x.InnerText)
                .ToArray();

            List<XmlNode> options = xmlDialogueList
                .Where(x => x.Name == "option" || x.Name == "option-end")
                .ToList();

            //Debug.Log("Count" + options.Count);

            DialogueOption[] dialogueOptions =
                options
                .Select(x => new DialogueOption(x.InnerText, x.Name == "option-end", x.Name == "option-end" ? null : x.Attributes["dialogue-id"].InnerText))
                .ToArray();


            dList.Add(options != null && options.Count > 0 ?
                new Dialogue(
                    id, 
                    speaker, 
                    endsConversation,
                    dialogueOptions //options.Select(x => new DialogueOption(x.InnerText, x.Name == "option-end", x.Attributes["dialogue-id"].InnerText)).ToArray()
                    )
                :
                new Dialogue(
                    id, 
                    speaker, 
                    endsConversation
                    )
                );
        }

        return new DialogueTree(dialogueTree.Attributes["id"].InnerText, dialogueTree.Attributes["name"].InnerText, dList.ToArray());
    }
}

public class DialogueObject 
{
    public string Speaker;
    public DialogueTree[] DialogueTrees;

    public DialogueObject(string speaker, DialogueTree[] dialogueTrees) 
    {
        Speaker = speaker;
        DialogueTrees = dialogueTrees;
    }
}

public class DialogueTree
{
    public string Id;
    public string Name;
    public Dictionary<string, Dialogue> Dialogues;

    public DialogueTree(string id, string name, Dialogue[] dialogues) 
    {
        Id = id;
        Name = name;
        if(dialogues != null && dialogues.Length > 0) Dialogues = dialogues.ToDictionary(x => x.Id, x => x);
    }
}

public class Dialogue 
{
    public string Id;
    public string[] Speaker;
    public bool EndsConversation;
    public DialogueOption[] DialogueOptions;

    public Dialogue(string id, string[] speaker, bool endsConversation, DialogueOption[] dialogueOptions) 
    {
        Id = id;
        Speaker = speaker;
        EndsConversation = endsConversation;
        DialogueOptions = dialogueOptions;
    }

    public Dialogue(string id, string[] speaker, bool endsConversation)
    {
        Id = id;
        Speaker = speaker;
        EndsConversation = endsConversation;
    }
}

public class DialogueOption 
{
    public string OptionText;
    public bool EndsConversation;
    public string DialogueID;

    public DialogueOption(string optionText, bool endsConversation, string dialogueId) 
    {
        OptionText = optionText;
        EndsConversation = endsConversation;
        DialogueID = dialogueId;
    }
}

//I don't know if this should be an instantiable class?

//public class DialogueManager
//{
//    private static DialogueManager dialogueManager;

//    private DialogueTree currentDialogueTree;
//    private Dialogue currentDialogue;
//    private Dialogue lastDialogue;

//    private string lastSpeakerDialogue = "";

//    public DialogueTree CurrentDialogueTree
//    {
//        get
//        {
//            return currentDialogueTree;
//        }
//        set
//        {
//            if (value != currentDialogueTree)
//            {
//                currentDialogueTree = value;
//                currentDialogue = value.Dialogues["1"];
//                lastDialogue = null;
//                lastSpeakerDialogue = "";
//            }
//        }
//    }

//    public Dialogue CurrentDialogue 
//    {
//        get { return currentDialogue; }
//    }

//    public static DialogueManager GetManager() 
//    {
//        if (dialogueManager == null) dialogueManager = new DialogueManager();
//        return dialogueManager;
//    }

//    public string GetDialogue() 
//    {
//        string dialogueString = "";
//        if (currentDialogue != null)
//        {

//            //Add speaker dialogue
//            if (currentDialogue.Speaker.Length > 0)
//            {
//                foreach (string s in currentDialogue.Speaker)
//                {
//                    dialogueString += s + "\n";
//                    lastSpeakerDialogue = s;
//                }
//                dialogueString += "\n";
//            }
//            else 
//            {
//                dialogueString += lastSpeakerDialogue + "\n";
//            }

//            //Add option dialogue
//            if (currentDialogue.DialogueOptions != null && currentDialogue.DialogueOptions.Length > 0) 
//            {
//                for (int i = 0; i < currentDialogue.DialogueOptions.Length; i++) 
//                {
//                    dialogueString += "(" + (i+1) +"). " + currentDialogue.DialogueOptions[i].OptionText + "\n";
//                }
//            }


//        }

//        return dialogueString;
//    }

//    public string SelectDialogueOption(int option) 
//    {
//        if (!currentDialogue.EndsConversation && currentDialogue.DialogueOptions != null && (option < currentDialogue.DialogueOptions.Length || option >= 0))
//        {
//            //If current conversation dialogue is then an end then end the conversation
//            if (currentDialogue.EndsConversation) return "conversation has ended";
            
//            //Get the selected dialogue option
//            DialogueOption selectedDialogueOption = currentDialogue.DialogueOptions[option];


//            if (selectedDialogueOption.EndsConversation)
//            {
                
//                return "ends"; //Conversation ends
//            }
//            else
//            {
//                //Get the intended new dialogue
//                Dialogue newDialogue = currentDialogueTree.Dialogues[selectedDialogueOption.DialogueID];

//                //If this ends the conversation then set it to current
//                if (newDialogue.EndsConversation) 
//                {
//                    currentDialogue = newDialogue;
//                }
//                else if (newDialogue.DialogueOptions == null || newDialogue.DialogueOptions.Length <= 0) //If this dialogue has no options then use the previous options
//                {
//                    currentDialogue = new Dialogue(newDialogue.Id, newDialogue.Speaker, newDialogue.EndsConversation, currentDialogue.DialogueOptions);
//                }
//                else 
//                {
//                    currentDialogue = newDialogue;
//                }

//                //currentDialogue = currentDialogueTree.Dialogues[selectedDialogueOption.DialogueID];
//                return GetDialogue();
//            }



//            //currentDialogue = currentDialogueTree.Dialogues[c]
//        }

//        return ""; //Im gonna do this tomorrow cba
//    }

//    private DialogueManager() 
//    {

//    }
//}
