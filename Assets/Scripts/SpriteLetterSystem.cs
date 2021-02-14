using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteLetterSystem : MonoBehaviour
{
    public Texture2D charSheet;
    public Dictionary<char, CharData> loadedFont;
    private void Awake()
    {
        loadedFont = FontLoader.LoadFontResource(charSheet);

        //https://youtu.be/jhwfA-QF54M?t=155 I wish this video was actually a tutorial
        //This is for testing that the characters are saved properly
        
        
        
        
        
        
        
        
        List<string> charAndLength = new List<string>();

        foreach (KeyValuePair<char, CharData> cD in loadedFont)
        {
            //Debug.Log("Char: " + cD.Key + ", CharWidth: " + cD.Value.Width);
            charAndLength.Add("[" + cD.Key + cD.Value.Width + "]");
        }

        Debug.Log("Letter info: " + string.Join(", ", charAndLength));
    }






}
