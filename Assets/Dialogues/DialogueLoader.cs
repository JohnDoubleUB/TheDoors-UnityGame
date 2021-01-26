using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;

public static class DialogueLoader
{
    private static string topLevelObject = "dialogue-sequence";
    private static string dialogueTree = "dialogue-tree";
    
    private static string dialogue = "dialogue";

    //within dialogue
    private static string speaker = "speaker"; //The speaker of a dialogue
    private static string option = "option";
    private static string optionEndConvo = "option-end";
    //Attributes

    //Attributes: id, tree-name, dialogue-id


    public static void LoadDialogueFile(string path) //TODO: This should return something!
    {

        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Find top level xml node!
            XmlNode topLevelNode = doc.DocumentElement.SelectSingleNode("/dialogue-object");

            XmlNodeList xmlNodes = topLevelNode.SelectNodes("dialogue-tree");

            Debug.Log("Node count" + xmlNodes.Count);

            foreach (XmlNode xmln in xmlNodes) 
            {
                Debug.Log("Node name?: " + xmln.Name);
            }

            Debug.Log("Successfully opened");
        }
        else 
        {
            Debug.Log("failed to open");
        }
  
    }

    //TODO: run various data through this to make sure it works properly, and genrally cleanup this mess! I haven't run this yet but I think it mostly should work
    private static DialogueTree BuildDialogueTree(XmlNode dialogueTree) 
    {
        List<Dialogue> dList = new List<Dialogue>();
        //First loop

        XmlNodeList xmlDialogues = dialogueTree.SelectNodes("Dialogue");

        foreach (XmlNode xmlDialogue in xmlDialogues) 
        {
            List<XmlNode> xmlDialogueList = new List<XmlNode>(xmlDialogue.ChildNodes.Cast<XmlNode>());
            
            string id = xmlDialogue.Attributes["id"].InnerText;
            
            bool endsConversation = xmlDialogueList.Any(x => x.Name == "speaker-end");

            string[] speaker = xmlDialogueList
                .Where(x => x.Name == "speaker" || x.Name == "speaker-end")
                .Select(x => x.InnerText)
                .ToArray();

            DialogueOption[] dialogueOptions = xmlDialogueList
                .Where(x => x.Name == "option" || x.Name == "option-end")
                .Select(x => new DialogueOption(x.InnerText, x.Name == "option-end", x.Attributes["dialogue-id"].InnerText))
                .ToArray();

            dList.Add(new Dialogue(id, speaker, endsConversation, dialogueOptions));
        }

        return new DialogueTree(dialogueTree.Attributes["id"].InnerText, dialogueTree.Attributes["name"].InnerText, dList.ToArray());
    }
}

public class DialogueTree
{
    public string Id;
    public string Name;
    Dictionary<string, Dialogue> Dialogues;

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
