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
    private static List<string> allAddedFlags = new List<string>();


    private static readonly DialogueObject[] dialogueObjects = LoadAllDialogueObjects();  //Load all dialogue intially

    //Attributes: id, tree-name, dialogue-id
    public static DialogueObject[] DialogueObjects //This is where we intially load in all the dialogue for the game
    {
        get { return dialogueObjects; }
    }
    public static List<string> AllAddedFlags 
    { 
        get { return allAddedFlags; } 
    }


    private static DialogueObject[] LoadAllDialogueObjects()
    {
        //Find all filenames
        string[] files = Directory.GetFiles(DialoguePath, "*." + DialogueFileExtension);
        DialogueObject[] dialogueObjects = files.Select(fPath => LoadDialogueFile(fPath)).Where(x => x != null).ToArray();
        
        return dialogueObjects;
    }

    private static void AddDistinctAddedFlagsToList(params string[][] flags) 
    {
        if (flags == null) return;

        string[] allFlags = flags.Where(x => x != null).SelectMany(x => x).ToArray();

        foreach (string flag in allFlags) 
        {
            if (!allAddedFlags.Contains(flag)) allAddedFlags.Add(flag);
        }
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
                    //Retreive all the requiredflags, progressflags and actionflags "reqflags", "progflags" and "actflags" for dialogue options
                    XmlNode dOptionReqFlagsNode = dOption.Attributes["reqflags"];
                    XmlNode dOptionProgFlagsNode = dOption.Attributes["progflags"];
                    XmlNode dOptionActFlagsNode = dOption.Attributes["actflags"];

                    string[] dOptionReqFlags = dOptionReqFlagsNode != null ? dOptionReqFlagsNode.InnerText.Split(' ') : null;
                    string[] dOptionActFlags = dOptionActFlagsNode != null ? dOptionActFlagsNode.InnerText.Split(' ') : null;
                    string[] dOptionProgFlags = dOptionProgFlagsNode != null ? dOptionProgFlagsNode.InnerText.Split(' ') : null;

                    AddDistinctAddedFlagsToList(dOptionReqFlags, dOptionActFlags, dOptionProgFlags);

                    XmlNode dDialogueIdXml = dOption.Attributes["dialogue-id"];

                    bool isInvalidDialogueOption = dDialogueIdXml == null && dOption.Name != "option-end";

                    if (isInvalidDialogueOption) 
                        Debug.LogError("ERROR: Dialogue option containing text: \"" + dOption.InnerText + "\" leads nowhere (isn't an \"option-end\" tag or contains a \"dialogue-id\")");

                    return new DialogueOption(
                        isInvalidDialogueOption ? dOption.InnerText + "[INVALID DIALOGUE!]" : dOption.InnerText,
                        dOption.Name == "option-end",
                        dOption.Name == "option-end" || dDialogueIdXml == null ? null : dOption.Attributes["dialogue-id"].InnerText,
                        dOptionReqFlags,
                        dOptionProgFlags,
                        dOptionActFlags
                        );
                })
                .ToArray();

            //Retreive all the progressflags and actionflags "progflags" and "actflags" for dialogues
            XmlNode dialogueProgFlagsNode = xmlDialogue.Attributes["progflags"];
            XmlNode dialogueActFlagsNode = xmlDialogue.Attributes["actflags"];

            string[] dialogueActFlags = dialogueActFlagsNode != null ? dialogueActFlagsNode.InnerText.Split(' ') : null;
            string[] dialogueProgFlags = dialogueProgFlagsNode != null ? dialogueProgFlagsNode.InnerText.Split(' ') : null;

            AddDistinctAddedFlagsToList(dialogueActFlags, dialogueProgFlags);
            //AddDistinctAddedFlagsToList(dialogueActFlags);

            dList.Add(options != null && options.Count > 0 ?
                new Dialogue(
                    id,
                    speakerDialogues,
                    endsConversation,
                    dialogueOptions,
                    dialogueProgFlags,
                    dialogueActFlags
                    )
                :
                new Dialogue(
                    id,
                    speakerDialogues,
                    endsConversation,
                    dialogueProgFlags,
                    dialogueActFlags
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
        return speaker.Length > 1 ? speaker.Substring(0, 1).ToUpper() + speaker.Remove(0, 1) : speaker.ToUpper();
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

    public string[] ProgressFlags;
    public string[] ActionFlags;

    public Dialogue(Dialogue dialogue, DialogueOption[] retainedDialogueOptions) 
    {
        Id = dialogue.Id;
        EndsConversation = dialogue.EndsConversation;
        SpeakerDialogues = dialogue.SpeakerDialogues;
        DialogueOptions = retainedDialogueOptions;
        ProgressFlags = dialogue.ProgressFlags;
        ActionFlags = dialogue.ActionFlags;
    }

    public Dialogue(string id, SpeakerDialogue[] speakerDialogues, bool endsConversation, DialogueOption[] dialogueOptions, string[] progFlags, string[] actFlags)
    {
        Id = id;
        EndsConversation = endsConversation;
        SpeakerDialogues = speakerDialogues;
        DialogueOptions = dialogueOptions;
        ProgressFlags = progFlags;
        ActionFlags = actFlags;
    }

    public Dialogue(string id, SpeakerDialogue[] speakerDialogues, bool endsConversation, string[] progFlags, string[] actFlags)
    {
        Id = id;
        SpeakerDialogues = speakerDialogues;
        EndsConversation = endsConversation;
        ProgressFlags = progFlags;
        ActionFlags = actFlags;
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

    public string[] RequiredFlags;
    public string[] ProgressFlags;
    public string[] ActionFlags;

    public DialogueOption(string optionText, bool endsConversation, string dialogueId, string[] requiredFlags, string[] progressFlags, string[] actionFlags)
    {
        OptionText = optionText;
        EndsConversation = endsConversation;
        DialogueID = dialogueId;
        RequiredFlags = requiredFlags;
        ProgressFlags = progressFlags;
        ActionFlags = actionFlags;
    }
}

public class SpeakerDialogue
{
    public string Text;
    public int SpeakerId;
    public bool EndsConversation;

    //TODO: Add required flags for speaker dialogues
    public SpeakerDialogue(string text, int speakerId, bool endsConversation)
    {
        Text = text;
        SpeakerId = speakerId;
        EndsConversation = endsConversation;
    }
}
