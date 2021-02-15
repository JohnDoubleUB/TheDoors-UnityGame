using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    char[] tags = { 'w', 'f' };

    void Start()
    {
        string test = "test <w>test<w> test";

        string test2 = "test test test";

        Debug.Log("expected: " + test2);

        Tuple<string, List<TextEffectType>> detagged = FindTags(test);

        string[] effects = detagged.Item2.Select(x => x.ToString()).ToArray();
        Debug.Log("untagged: " + detagged.Item1 + " effects: " + string.Join<string>(", " , effects));
    }


    private Tuple<string, List<TextEffectType>> FindTags(string test) 
    {
        bool foundTag = false;
        //bool foundTagEnd = false;

        string untagged = "";
        char currentTag = ' ';
        List<TextEffectType> effectList = new List<TextEffectType>();
        
        
        for (int i = 0; i < test.Length; i++) 
        {
            char currentLetter = test[i];

            //Look for opening tag
            if (!foundTag && currentLetter == '<' && i + 2 < test.Length && char.IsLetter(test[i+1]) && test[i+2] == '>' ) 
            {
                foundTag = true;
                currentTag = test[i + 1];
                if (i + 3 < test.Length)
                {
                    i += 3;
                    currentLetter = test[i];
                }
                else 
                {
                    break;
                }
            }

            if (foundTag && currentLetter == '<' && i + 2 < test.Length && char.IsLetter(test[i + 1]) && test[i + 2] == '>') 
            {
                if (i + 3 < test.Length)
                {
                    i += 3;
                    currentLetter = test[i];
                    foundTag = false;
                    currentTag = ' ';
                }
                else
                {
                    break;
                }
            }

            effectList.Add(GetTagType(currentTag));

            untagged += currentLetter;

        }
        return new Tuple<string, List<TextEffectType>>(untagged, effectList);
    }

    private TextEffectType GetTagType(char tagLetter) 
    {
        switch (tagLetter) 
        {
            case 'w':
                return TextEffectType.Wavy;
            case 's':
                return TextEffectType.Stuttery;
            case 'r':
                return TextEffectType.Rainbow;
            default:
                return TextEffectType.None;
        }
    }
}



public enum TextEffectType 
{
    None,
    Wavy,
    Stuttery,
    Rainbow
}