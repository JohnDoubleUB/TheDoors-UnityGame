using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteLetterSystem : MonoBehaviour
{
    private char[] chars = "abcdefghijklmnopqrstuvwxyzæABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~><!?'\"#%&/\\()[]{}@£$*^+-.,:;_=".ToCharArray();
    private Sprite[] charSprites;
    public Dictionary<char, CharData> charData;
    public int spriteSize;

    public Texture2D charSheet;

    public void GetSubSprites()
    {
        Sprite[] subsprites = Resources.LoadAll<Sprite>(charSheet.name);
        charSprites = subsprites;
        spriteSize = charSheet.width / subsprites.Length;
    }

    public void GetSpriteWidths()
    {
        //int height = charSheet.height; // We might need this if we ever use a text image that is on more than one line
        int width = charSheet.width;

        int charIndex = 0;

        charData = new Dictionary<char, CharData>();

        int minY = 0;
        int maxY = minY + spriteSize;
        
        //Apparently GetPixel is weird and broken and when I use a private method to neaten things up unity just dies
        
        for (int texCoordX = 0; texCoordX < width; texCoordX += spriteSize)
        {
            int minX = texCoordX;
            int maxX = texCoordX + (spriteSize - 1);
            bool edgeFound = false;

            //right edge
            int rightEdge = 0;
            for (int currentX = maxX; currentX >= minX; currentX--)
            {
                for (int currentY = minY; currentY < maxY; currentY++)
                {
                    edgeFound = charSheet.GetPixel(currentX, currentY).a != 0;
                    if (edgeFound) break;
                }
                if (edgeFound) break;
                rightEdge++;
            }

            edgeFound = false;


            //left edge
            int leftEdge = 0;
            for (int currentX = minX; currentX <= maxX; currentX++)
            {
                //X
                for (int currentY = minY; currentY < maxY; currentY++)
                {
                    edgeFound = charSheet.GetPixel(currentX, currentY).a != 0;
                    if (edgeFound) break;
                }
                if (edgeFound) break;
                leftEdge++;
            }
            
            //Store current sprite width
            int currentSpriteWidth = spriteSize - (leftEdge + rightEdge);

            charData.Add(chars[charIndex], new CharData(currentSpriteWidth, charSprites[charIndex]));
            
            charIndex++;
        }
    }

    private void Awake()
    {
        GetSubSprites();
        GetSpriteWidths();

        //https://youtu.be/jhwfA-QF54M?t=155 I wish this video was actually a tutorial
        //This is for testing that the characters are saved properly
        //foreach (KeyValuePair<char, CharData> cD in charData) 
        //{
        //    Debug.Log("Char: " + cD.Key + ", CharWidth: " + cD.Value.Width);
        //}
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

    void Start()
    {

    }
}

public struct CharData
{
    public int Width;

    public Sprite Sprite;

    public CharData(int width, Sprite sprite)
    {
        Width = width;
        Sprite = sprite;
    }
}
