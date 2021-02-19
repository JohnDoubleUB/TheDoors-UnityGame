using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    string[] reservedTags = { "w", "f", "s", "re", "green", "blue" };

    void Start()
    {
        //string example = "<blue><green>test <w>test</w> test</green></blue>";
        ////              0123 5678 9
        //string test2 = "test test test";

        //Debug.Log("expected: " + test2);

        //TagFilteredDialogue2 result1 = ExtractTags(example);

        //Debug.Log("result: " + result1.UntaggedString);

        //foreach (KeyValuePair<string, int[]> tagPair in result1.StringTags) 
        //{
        //    Debug.Log(tagPair.Key + " tagged indexes: " + string.Join(", ", tagPair.Value));
        //}
    }


    private TagFilteredDialogue2 ExtractTags(string taggedString)
    {
        bool isClosingTag;

        string untaggedString = "";
        List<string> currentTags = new List<string>();

        //Used to check likely hood of tag quickly
        char[] tagStartingCharacters = reservedTags.Select(x => x[0]).ToArray();

        Dictionary<string, List<int>> stringTags = new Dictionary<string, List<int>>();

        for (int i = 0; i < taggedString.Length; i++)
        {
            //Look for tag
            if (taggedString[i] == '<' && i+2 < taggedString.Length)
            {
                //If the next letter is / we have a closing tag
                isClosingTag = taggedString[i + 1] == '/';
                
                //This determines where the inside of a tag starts (past both < and / if its a closing tag)
                int nextCharIndex = isClosingTag ? i + 2 : i + 1;

                //Check quickly if this is likely to be a tag because this would be silly to check otherwise
                //Check which tags it could be
                if(tagStartingCharacters.Contains(taggedString[nextCharIndex]))
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

                                if (!stringTags.ContainsKey(match)) stringTags.Add(match, new List<int>());

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
                    foreach (string t in currentTags) stringTags[t].Add(currentNewIndex);
                }
            }
        }

        return new TagFilteredDialogue2(
            untaggedString, 
            stringTags.ToDictionary(x => x.Key, x => x.Value.ToArray())
            );
    }


    private struct TagFilteredDialogue2 
    {
        public string UntaggedString;
        public Dictionary<string, int[]> StringTags;
        public TagFilteredDialogue2(string untaggedString, Dictionary<string, int[]> stringTags) 
        {
            UntaggedString = untaggedString;
            StringTags = stringTags;
        }
    }
}