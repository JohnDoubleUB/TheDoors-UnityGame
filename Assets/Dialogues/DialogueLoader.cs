using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;

public static class DialogueLoader
{
    private static string DialoguePath = Application.dataPath + "/Dialogues/";
    private static string DialogueFileExtension = "xml";
    private static readonly DialogueObject[] dialogueObjects = LoadAllDialogueObjects();  //Load all dialogue intially


    //Attributes: id, tree-name, dialogue-id
    public static DialogueObject[] DialogueObjects { get; } = dialogueObjects; //This is where we intially load in all the dialogue for the game


    private static DialogueObject[] LoadAllDialogueObjects()
    {
        //Find all filenames
        string[] files = Directory.GetFiles(DialoguePath, "*." + DialogueFileExtension);
        return files.Select(fPath => LoadDialogueFile(fPath)).Where(x => x != null).ToArray();
    }


    private static DialogueObject LoadDialogueFile(string path)
    {
        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path); //I think this can throw an error if the xml is malformed?

                //Find top level xml node!
                XmlNode topLevelNode = doc.DocumentElement.SelectSingleNode("/dialogue-object");

                XmlNodeList xmlNodes = topLevelNode.SelectNodes("dialogue-tree");

                DialogueTree[] dialogueTrees = new List<XmlNode>(xmlNodes.Cast<XmlNode>()).Select(x => BuildDialogueTree(x)).ToArray();

                string[] speakers = topLevelNode.Attributes["speakers"].InnerText.Split(' ');

                return new DialogueObject(speakers, dialogueTrees, Path.GetFileNameWithoutExtension(path));
            }
            catch (XmlException e)
            {
                Debug.LogError("ERROR: Malformed XML in file: " + path + ", this file will be skipped, Full stack trace: " + e);
                return null;
            }
        }
        else
        {
            Debug.LogError("FILE MISSING: file at path: " + path + " doesn't exist!");

            return null;
        }

    }

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

            SpeakerDialogue[] speakerDialogues = xmlDialogueList
                .Where(x => x.Name == "speaker" || x.Name == "speaker-end")
                .Select(x => new SpeakerDialogue(x.InnerText, GetSpeakerId(x), x.Name == "speaker-end"))
                .ToArray();

            List<XmlNode> options = xmlDialogueList
                .Where(x => x.Name == "option" || x.Name == "option-end")
                .ToList();

            DialogueOption[] dialogueOptions =
                options
                .Select(dOption => 
                {
                    //TODO: Retreive all the requiredflags, progressflags and actionflags "reqflag", "progflag" and "actflag"

                    
                    return new DialogueOption(dOption.InnerText, dOption.Name == "option-end", dOption.Name == "option-end" ? null : dOption.Attributes["dialogue-id"].InnerText);
                })
                .ToArray();


            dList.Add(options != null && options.Count > 0 ?
                new Dialogue(
                    id,
                    speakerDialogues,
                    endsConversation,
                    dialogueOptions
                    )
                :
                new Dialogue(
                    id,
                    speakerDialogues,
                    endsConversation
                    )
                );
        }

        return new DialogueTree(dialogueTree.Attributes["id"].InnerText, dialogueTree.Attributes["name"].InnerText, dList.ToArray());
    }

    private static int GetSpeakerId(XmlNode xmlNode) 
    {
        XmlNode speakerIndexXml = xmlNode.Attributes["speaker-index"];

        string speakerIndexText = speakerIndexXml != null ? speakerIndexXml.InnerText : null;

        return !string.IsNullOrEmpty(speakerIndexText) ? 
            int.TryParse(speakerIndexText, out int speakerIndex) ? 
            speakerIndex : 0 : 0;
    }
}

public class DialogueObject
{
    public string[] Speakers;
    public string Name;
    public DialogueTree[] DialogueTrees;

    public string GetSpeakerWithCapital(int index) 
    {
        string speaker = Speakers[index];
        return speaker.Length > 1 ? speaker.Substring(0,1).ToUpper() + speaker.Remove(0, 1) : speaker.ToUpper();
    }

    public DialogueObject(string[] speakers, DialogueTree[] dialogueTrees, string name)
    {
        Speakers = speakers;
        DialogueTrees = dialogueTrees;
        Name = name;
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
        if (dialogues != null && dialogues.Length > 0) Dialogues = dialogues.ToDictionary(x => x.Id, x => x);
    }
}

public class Dialogue
{
    public string Id;
    public SpeakerDialogue[] SpeakerDialogues;
    public bool EndsConversation;
    public DialogueOption[] DialogueOptions;

    /* 
     * --:NOTES ON FLAGS FOR DIALOGUES:--
     * ActionFlags - These activate after this dialogue is left
     * ProgressFlags - Just stores this flag in your save, it might be useful later, who knows? its for progress tracking mostly but in particular related to conversations
     */

    //TODO: Get these flags working!
    public string[] ActionFlags;
    public string[] ProgressFlags;

    public Dialogue(string id, SpeakerDialogue[] speakerDialogues, bool endsConversation, DialogueOption[] dialogueOptions)
    {
        Id = id;
        EndsConversation = endsConversation;
        DialogueOptions = dialogueOptions;
        SpeakerDialogues = speakerDialogues;
    }

    public Dialogue(string id, SpeakerDialogue[] speakerDialogues, bool endsConversation)
    {
        Id = id;
        EndsConversation = endsConversation;
        SpeakerDialogues = speakerDialogues;
    }
}

public class DialogueOption
{
    public string OptionText;
    public bool EndsConversation;
    public string DialogueID;

    /* 
     * --:NOTES ON FLAGS FOR DIALOGUE OPTIONS:--
     * ActionFlags - These activate immediately once an option is clicked which has one, it usually starts an event of somesort
     * ProgressFlags - Just stores this flag in your save, it might be useful later, who knows? its for progress tracking mostly but in particular related to conversations
     * RequiredFlags - This is unique to dialogue options, if there is a required flag then this flag must be present in the players flags in order to allow this option to appear
     */

    //TODO: Get these flags working!
    public string[] ActionFlags;
    public string[] ProgressFlags;
    public string[] RequiredFlags;

    public DialogueOption(string optionText, bool endsConversation, string dialogueId)
    {
        OptionText = optionText;
        EndsConversation = endsConversation;
        DialogueID = dialogueId;
    }
}

public class SpeakerDialogue 
{
    public string Text;
    public int SpeakerId;
    public bool EndsConversation;

    public SpeakerDialogue(string text, int speakerId, bool endsConversation) 
    {
        Text = text;
        SpeakerId = speakerId;
        EndsConversation = endsConversation;
    }
}
