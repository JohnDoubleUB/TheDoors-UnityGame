using System.Collections;
using System.Collections.Generic;
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
        Dictionary<char, CharData> testThing2 = FontLoader.LoadFontResource(charSheet);

        //https://youtu.be/jhwfA-QF54M?t=155 I wish this video was actually a tutorial
        //This is for testing that the characters are saved properly
        foreach (KeyValuePair<char, CharData> cD in testThing)
        {
            Debug.Log("Char: " + cD.Key + ", CharWidth: " + cD.Value.Width);
        }
    }


    //public void GetSpriteWidthsNope()
    //{
    //int height = charSheet.height;
    //int width = charSheet.width;

    //int charIndex = 0;

    //charData = new Dictionary<char, CharData>();
    //for (int texCoordY = 0; texCoordY > 0; texCoordY -= spriteSize) 
    //{
    //    for (int texCoordX = 0; texCoordX < width; texCoordX += spriteSize)
    //    {
    //        int x = 0;
    //        int y = 0;

    //        int min = 0;
    //        int max = spriteSize;

    //        while (min == 0 && x < spriteSize) 
    //        {
    //            for (y = 0; y < spriteSize; y++) 
    //            {
    //                if (charSheet.GetPixel(texCoordX + x, texCoordY - y).a != 0) 
    //                {
    //                    min = x;
    //                }
    //            }
    //        }

    //        x = spriteSize;

    //        while (max == spriteSize && x > 0) 
    //        {
    //            for (y = 0; y < spriteSize; y++) 
    //            {
    //                if (charSheet.GetPixel(texCoordX + x, texCoordY + y).a != 0) 
    //                {
    //                    max = x;
    //                }
    //            }
    //            x--;
    //        }

    //        int charWidth = max - min + 1;
    //        if (charWidth <= spriteSize && charIndex < chars.Length) 
    //        {
    //            Debug.Log("hi");
    //            char c = chars[charIndex];
    //            Sprite charSprite = charSprites[charIndex];

    //            charData.Add(c, new CharData(charWidth, charSprite));
    //        }
    //    }
    //}
    //}
}
