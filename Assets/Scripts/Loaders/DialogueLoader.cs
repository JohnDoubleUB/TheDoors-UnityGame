using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;
using System;

public class DialogueLoader : XmlLoader
{
    public static DialogueLoader current = new DialogueLoader();

    private string[] reservedTags = { "w", "f", "s", "c", "r", "g", "b", "y" };
    private List<string> allAddedFlags = new List<string>();
    private string dialogueStartId = "1"; //This is essentially the default entrypoint to a given dialogue tree
    private readonly DialogueObject[] dialogueObjects;  //Load all dialogue intially

    //Attributes: id, tree-name, dialogue-id
    //NOTE: DialogueObject Name are now considered to be DialogueTreeFlags! Separate from flags!

    private DialogueLoader() 
    {
        FilePath = "Dialogues";
        TopLevelNode = "dialogue-object";
        dialogueObjects = LoadAllDialogueObjects();
    }

    public DialogueObject[] DialogueObjects //This is where we intially load in all the dialogue for the game
    {
        get { return dialogueObjects; }
    }
    public List<string> AllAddedFlags 
    { 
        get { return allAddedFlags; } 
    }

    public string DialogueStartId 
    {
        get { return dialogueStartId; }
    }

    public string[] ReservedTags 
    {
        get { return reservedTags; }
    }

    private DialogueObject[] LoadAllDialogueObjects()
    {
        DialogueObject[] dialogueObjects = LoadAllXmlFiles().Select(xFile => LoadDialogueFile(xFile)).ToArray();        
        return dialogueObjects;
    }

    private void AddDistinctAddedFlagsToList(params string[][] flags) 
    {
        if (flags == null) return;

        string[] allFlags = flags.Where(x => x != null).SelectMany(x => x).ToArray();

        foreach (string flag in allFlags) 
        {
            if (!allAddedFlags.Contains(flag)) allAddedFlags.Add(flag);
        }
    }

    private DialogueObject LoadDialogueFile(LoadedXmlFile file)
    {
            try
            {
                XmlNode topLevelNode = file.Node;

                XmlNodeList xmlNodes = topLevelNode.SelectNodes("dialogue-tree");

                DialogueTree[] dialogueTrees = new List<XmlNode>(xmlNodes.Cast<XmlNode>()).Select(x => BuildDialogueTree(x)).ToArray();

                string[] speakers = topLevelNode.Attributes["speakers"].InnerText.Split(' ');

                return new DialogueObject(speakers, dialogueTrees, file.Name);
            }
            catch (XmlException e)
            {
                Debug.LogError("ERROR: Malformed Dialogue XML in file: " + file.Name + ", this file will be skipped, Full stack trace: " + e);
                return null;
            }

    }

    private DialogueTree BuildDialogueTree(XmlNode dialogueTree)
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
                .Select(sDialogue => 
                {
                    XmlNode sDialogueReqFlagsNode = sDialogue.Attributes["reqflags"];
                    XmlNode sDialogueUnReqFlagsNode = sDialogue.Attributes["unreqflags"];

                    string[] sDialogueReqFlags = sDialogueReqFlagsNode != null ? sDialogueReqFlagsNode.InnerText.Split(' ') : null;
                    string[] sDialogueUnReqFlags = sDialogueUnReqFlagsNode != null ? sDialogueUnReqFlagsNode.InnerText.Split(' ') : null;

                    AddDistinctAddedFlagsToList(sDialogueReqFlags, sDialogueUnReqFlags);
                    //Change sDialogue.InnerText to sDialogue.InnerXML?
                    //Filter here? Hopefully this doesn't go horribly wrong yeah?
                    TagFilteredDialogue tagFilteredDialogue = ExtractTags(sDialogue.InnerXml);

                    return new SpeakerDialogue(tagFilteredDialogue.UntaggedString, GetSpeakerId(sDialogue), sDialogue.Name == "speaker-end", sDialogueReqFlags, sDialogueUnReqFlags, tagFilteredDialogue.IndexedTags);
                })
                .ToArray();

            List<XmlNode> options = xmlDialogueList
                .Where(x => x.Name == "option" || x.Name == "option-end")
                .ToList();

            DialogueOption[] dialogueOptions =
                options
                .Select(dOption =>
                {
                    //Retreive all the requiredflags, progressflags, actionflags, unrequiredflags and temporaryactionflags; "reqflags", "progflags" and "actflags", "unreqflags" and "tempactflags" for dialogue options
                    XmlNode dOptionReqFlagsNode = dOption.Attributes["reqflags"];
                    XmlNode dOptionProgFlagsNode = dOption.Attributes["progflags"];
                    XmlNode dOptionActFlagsNode = dOption.Attributes["actflags"];
                    XmlNode dOptionUnReqFlagsNode = dOption.Attributes["unreqflags"];
                    //TemporaryActionFlags
                    XmlNode dOptionTempActFlagsNode = dOption.Attributes["tempactflags"];

                    string[] dOptionReqFlags = dOptionReqFlagsNode != null ? dOptionReqFlagsNode.InnerText.Split(' ') : null;
                    string[] dOptionActFlags = dOptionActFlagsNode != null ? dOptionActFlagsNode.InnerText.Split(' ') : null;
                    string[] dOptionProgFlags = dOptionProgFlagsNode != null ? dOptionProgFlagsNode.InnerText.Split(' ') : null;
                    string[] dOptionUnReqFlags = dOptionUnReqFlagsNode != null ? dOptionUnReqFlagsNode.InnerText.Split(' ') : null;
                    //TemporaryActionFlags
                    string[] dOptionTempActFlags = dOptionTempActFlagsNode != null ? dOptionTempActFlagsNode.InnerText.Split(' ') : null;

                    AddDistinctAddedFlagsToList(dOptionReqFlags, dOptionActFlags, dOptionProgFlags, dOptionUnReqFlags);

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
                        dOptionActFlags,
                        dOptionUnReqFlags,
                        dOptionTempActFlags
                        );
                })
                .ToArray();

            //Retreive all the progressflags, actionflags and tempactflags "progflags", "actflags" and "tempactflags" for dialogues
            XmlNode dialogueProgFlagsNode = xmlDialogue.Attributes["progflags"];
            XmlNode dialogueActFlagsNode = xmlDialogue.Attributes["actflags"];
            //TemporaryActionFlags
            XmlNode dialogueTempActFlagsNode = xmlDialogue.Attributes["tempactflags"];

            string[] dialogueActFlags = dialogueActFlagsNode != null ? dialogueActFlagsNode.InnerText.Split(' ') : null;
            string[] dialogueProgFlags = dialogueProgFlagsNode != null ? dialogueProgFlagsNode.InnerText.Split(' ') : null;
            //TemporaryActionFlags
            string[] dialogueTempActFlags = dialogueTempActFlagsNode != null ? dialogueTempActFlagsNode.InnerText.Split(' ') : null;

            AddDistinctAddedFlagsToList(dialogueActFlags, dialogueProgFlags);

            dList.Add(options != null && options.Count > 0 ?
                new Dialogue(
                    id,
                    speakerDialogues,
                    endsConversation,
                    dialogueOptions,
                    dialogueProgFlags,
                    dialogueActFlags,
                    dialogueTempActFlags
                    )
                :
                new Dialogue(
                    id,
                    speakerDialogues,
                    endsConversation,
                    dialogueProgFlags,
                    dialogueActFlags,
                    dialogueTempActFlags
                    )
                );
        }

        return new DialogueTree(dialogueTree.Attributes["id"].InnerText, dialogueTree.Attributes["name"].InnerText, dList.ToArray());
    }

    private int GetSpeakerId(XmlNode xmlNode)
    {
        XmlNode speakerIndexXml = xmlNode.Attributes["speaker-index"];

        string speakerIndexText = speakerIndexXml != null ? speakerIndexXml.InnerText : null;

        return !string.IsNullOrEmpty(speakerIndexText) ?
            int.TryParse(speakerIndexText, out int speakerIndex) ?
            speakerIndex : 0 : 0;
    }


    private TagFilteredDialogue ExtractTags(string taggedString)
    {
        bool isClosingTag;

        string untaggedString = "";
        List<string> currentTags = new List<string>();

        //Used to check likely hood of tag quickly
        char[] tagStartingCharacters = reservedTags.Select(x => x[0]).ToArray();

        Dictionary<int, string[]> indexedTags = new Dictionary<int, string[]>();

        for (int i = 0; i < taggedString.Length; i++)
        {
            //Look for tag
            if (taggedString[i] == '<' && i + 2 < taggedString.Length)
            {
                //If the next letter is / we have a closing tag
                isClosingTag = taggedString[i + 1] == '/';

                //This determines where the inside of a tag starts (past both < and / if its a closing tag)
                int nextCharIndex = isClosingTag ? i + 2 : i + 1;

                //Check quickly if this is likely to be a tag because this would be silly to check otherwise
                //Check which tags it could be
                if (tagStartingCharacters.Contains(taggedString[nextCharIndex]))
                {
                    foreach (string reservedTag in reservedTags)
                    {
                        if (nextCharIndex + reservedTag.Length < taggedString.Length)
                        {
                            string match = taggedString.Substring(nextCharIndex, reservedTag.Length);

                            if (taggedString[nextCharIndex + reservedTag.Length] == '>' && reservedTag.Contains(match))
                            {
                                if (isClosingTag) currentTags.Remove(match);
                                else currentTags.Add(match);

                                i = nextCharIndex + reservedTag.Length + 1;
                                break;
                            }
                        }
                    }
                }
            }

            if (i < taggedString.Length)
            {
                if (taggedString[i] == '<' && i + 2 < taggedString.Length)
                {
                    i--;
                }
                else
                {
                    untaggedString += taggedString[i];
                    int currentNewIndex = untaggedString.Length - 1;
                    if (currentTags.Any() && taggedString[i] != ' ') indexedTags.Add(currentNewIndex, currentTags.ToArray());
                }
            }
        }

        return new TagFilteredDialogue(
            untaggedString,
            indexedTags
            );
    }


    private struct TagFilteredDialogue
    {
        public string UntaggedString;
        public Dictionary<int, string[]> IndexedTags;
        public TagFilteredDialogue(string untaggedString, Dictionary<int, string[]> indexedTags)
        {
            UntaggedString = untaggedString;
            IndexedTags = indexedTags;
        }
    }

}

public class DialogueObject
{
    public string[] Speakers;
    public string Name;
    public DialogueTree[] DialogueTrees;

    public string GetSpeakerNiceName(int index)
    {
        string speaker = Speakers[index];
        return speaker.Length > 1 ? string.Join(" ", speaker.Split('_').Select(x => x.Substring(0, 1).ToUpper() + x.Remove(0, 1))) : speaker.ToUpper();
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

    public Dialogue EntryPoint 
    {
        get 
        {
            return Dialogues.ContainsKey(DialogueLoader.current.DialogueStartId) ? Dialogues[DialogueLoader.current.DialogueStartId] : null; 
        }
    }

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
     * TemporaryActionFlags - Like action flags, except that they are never stored as flags an instead are just triggers
     */

    public string[] ProgressFlags;
    public string[] ActionFlags;
    public string[] TemporaryActionFlags;

    public Dialogue(Dialogue dialogue, DialogueOption[] dialogueOptions) 
    {
        Id = dialogue.Id;
        EndsConversation = dialogue.EndsConversation;
        SpeakerDialogues = dialogue.SpeakerDialogues;
        DialogueOptions = dialogueOptions;
        ProgressFlags = dialogue.ProgressFlags;
        ActionFlags = dialogue.ActionFlags;
        TemporaryActionFlags = dialogue.TemporaryActionFlags;
    }

    public Dialogue(Dialogue dialogue, SpeakerDialogue[] speakerDialogues) 
    {
        Id = dialogue.Id;
        EndsConversation = dialogue.EndsConversation;
        SpeakerDialogues = speakerDialogues;
        DialogueOptions = dialogue.DialogueOptions;
        ProgressFlags = dialogue.ProgressFlags;
        ActionFlags = dialogue.ActionFlags;
        TemporaryActionFlags = dialogue.TemporaryActionFlags;
    }

    public Dialogue(Dialogue dialogue, SpeakerDialogue[] speakerDialogues, DialogueOption[] dialogueOptions)
    {
        Id = dialogue.Id;
        EndsConversation = dialogue.EndsConversation;
        SpeakerDialogues = speakerDialogues;
        DialogueOptions = dialogueOptions;
        ProgressFlags = dialogue.ProgressFlags;
        ActionFlags = dialogue.ActionFlags;
        TemporaryActionFlags = dialogue.TemporaryActionFlags;
    }

    public Dialogue(string id, SpeakerDialogue[] speakerDialogues, bool endsConversation, DialogueOption[] dialogueOptions, string[] progFlags, string[] actFlags, string[] tempActFlags)
    {
        Id = id;
        EndsConversation = endsConversation;
        SpeakerDialogues = speakerDialogues;
        DialogueOptions = dialogueOptions;
        ProgressFlags = progFlags;
        ActionFlags = actFlags;
        TemporaryActionFlags = tempActFlags;
    }

    public Dialogue(string id, SpeakerDialogue[] speakerDialogues, bool endsConversation, string[] progFlags, string[] actFlags, string[] tempActFlags)
    {
        Id = id;
        SpeakerDialogues = speakerDialogues;
        EndsConversation = endsConversation;
        ProgressFlags = progFlags;
        ActionFlags = actFlags;
        TemporaryActionFlags = tempActFlags;
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
     * UnRequiredFlags - Unique to dialogue options, if one of these flags is present then this option will not appear
     * TemporaryActionFlags - Like action flags, except that they are never stored as flags an instead are just triggers
     */

    public string[] RequiredFlags;
    public string[] ProgressFlags;
    public string[] ActionFlags;
    public string[] UnRequiredFlags;
    public string[] TemporaryActionFlags;

    public DialogueOption(string optionText, bool endsConversation, string dialogueId, string[] requiredFlags, string[] progressFlags, string[] actionFlags, string[] unRequiredFlags, string[] temporaryActionFlags)
    {
        OptionText = optionText;
        EndsConversation = endsConversation;
        DialogueID = dialogueId;
        RequiredFlags = requiredFlags;
        ProgressFlags = progressFlags;
        ActionFlags = actionFlags;
        UnRequiredFlags = unRequiredFlags;
        TemporaryActionFlags = temporaryActionFlags;
    }
}

public class SpeakerDialogue
{
    public string Text;
    public int SpeakerId;
    public bool EndsConversation;
    public Dictionary<int, string[]> DialogueEffects;

    /* 
     * --:NOTES ON FLAGS FOR SPEAKER DIALOGUE:--
     * RequiredFlags - This is unique to dialogue options, if there is a required flag then this flag must be present in the players flags in order to allow this option to appear
     * UnRequiredFlags - Unique to dialogue options, if one of these flags is present then this option will not appear
     */

    public string[] RequiredFlags;
    public string[] UnRequiredFlags;

    //public SpeakerDialogue(string text, int speakerId, bool endsConversation, string[] requiredFlags, string[] unRequiredFlags)
    //{
    //    Text = text;
    //    SpeakerId = speakerId;
    //    EndsConversation = endsConversation;
    //    RequiredFlags = requiredFlags;
    //    UnRequiredFlags = unRequiredFlags;
    //}
    public SpeakerDialogue(string text, int speakerId, bool endsConversation, string[] requiredFlags, string[] unRequiredFlags, Dictionary<int, string[]> dialogueEffects = null)
    {
        Text = text;
        SpeakerId = speakerId;
        EndsConversation = endsConversation;
        RequiredFlags = requiredFlags;
        UnRequiredFlags = unRequiredFlags;
        DialogueEffects = dialogueEffects;
    }

}
