using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

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


    public static void LoadDialogueFile(string path) 
    {

        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            string attr;

            //Find top level xml node!
            XmlNode topLevelNode = doc.DocumentElement.SelectSingleNode("/dialogue-object");

            XmlNodeList xmlNodes = topLevelNode.SelectNodes("dialoguetree");

            Debug.Log("Node count" + xmlNodes.Count);

            foreach (XmlNode xmln in xmlNodes) 
            {
                attr = xmln.Attributes["tree-name"]?.InnerText;
                if (!string.IsNullOrEmpty(attr)) Debug.Log(attr);
            }

            Debug.Log("Successfully opened");
        }
        else 
        {
            Debug.Log("failed to open");
        }
  
    }
}

public class dialogueSequence 
{

}
