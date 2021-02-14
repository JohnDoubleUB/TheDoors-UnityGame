using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteLetterSystem : MonoBehaviour
{
    public Texture2D charSheet;

    /* 
     * Notes for future reference:
     * Texture2D objects .GetPixel() is janky as hell and can cause unity to crash or freeze (its great) so be aware.
     * If you're image is larger than its set MaxSize in the inspector then set it up so that this doesn't change the size of the loaded image, if you don't you'll have a bad time
     * Resources.LoadAll<>() it loads assets from a Resources folder, this can be in various places but its probably best to put it in your Assets folder
     * Don't be tempted to use AssetDatabase because that only works in the editor and if your game compiled requires this you'll be in trouble
     * Check your image is loaded at the right resolution properly first, I didn't and it was the last thing I ended up checking.
     */

    private void Awake()
    {
        Dictionary<char, CharData> testThing = FontLoader.LoadFontResource(charSheet);

        //https://youtu.be/jhwfA-QF54M?t=155 I wish this video was actually a tutorial
        //This is for testing that the characters are saved properly
        List<string> charAndLength = new List<string>();

        foreach (KeyValuePair<char, CharData> cD in testThing)
        {
            //Debug.Log("Char: " + cD.Key + ", CharWidth: " + cD.Value.Width);
            charAndLength.Add("[" + cD.Key + cD.Value.Width + "]");
        }

        Debug.Log("Letter info: " + string.Join(", ", charAndLength));
    }
}
