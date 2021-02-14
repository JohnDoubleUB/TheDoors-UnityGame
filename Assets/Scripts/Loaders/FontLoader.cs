using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FontLoader
{
    //This is the order that the characters should be in the characterSheet
    private static char[] chars = "abcdefghijklmnopqrstuvwxyzæABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~><!?'\"#%&/\\()[]{}@£$*^+-.,:;_=".ToCharArray();
    private static List<Dictionary<char, CharData>> loadedFonts = new List<Dictionary<char, CharData>>(); //Stores all loaded fonts
    private static List<Texture2D> loadedFontResources = new List<Texture2D>(); //I'm going to use this to check if we already have a resource loaded

    /* 
     * Notes for future reference:
     * Texture2D objects .GetPixel() is janky as hell and can cause unity to crash or freeze (its great) so be aware.
     * If you're image is larger than its set MaxSize in the inspector then set it up so that this doesn't change the size of the loaded image, if you don't you'll have a bad time
     * Resources.LoadAll<>() it loads assets from a Resources folder, this can be in various places but its probably best to put it in your Assets folder
     * Don't be tempted to use AssetDatabase because that only works in the editor and if your game compiled requires this you'll be in trouble
     * Check your image is loaded at the right resolution properly first, I didn't and it was the last thing I ended up checking.
     */

    public static List<Dictionary<char, CharData>> LoadedFonts 
    {
        get { return loadedFonts; }
    }

    public static List<Dictionary<char, CharData>> LoadFontResources(params Texture2D[] characterSheet) 
    {
        return characterSheet != null && characterSheet.Any() ? 
            characterSheet.Select(x => LoadFontResource(x)).ToList() : 
            new List<Dictionary<char, CharData>>();
    }
    public static List<Dictionary<char, CharData>> ReloadFontResources(params Texture2D[] characterSheet)
    {
        return characterSheet != null && characterSheet.Any() ?
            characterSheet.Select(x => ReloadFontResource(x)).ToList() :
            new List<Dictionary<char, CharData>>();
    }


    public static Dictionary<char, CharData> LoadFontResource(Texture2D characterSheet) 
    {
        return LoadFontResource(characterSheet, true);
    }

    private static Dictionary<char, CharData> LoadFontResource(Texture2D characterSheet, bool addToLoaded)
    {
        //If we already have this loaded then we just return the loaded one
        if (IsFontLoaded(characterSheet)) return loadedFonts[loadedFontResources.IndexOf(characterSheet)];

        Sprite[] subsprites = Resources.LoadAll<Sprite>(characterSheet.name);
        int spriteSize = characterSheet.width / subsprites.Length;

        Dictionary<char, CharData>  loadedFontDictionary = GenerateCharFontDictionary(characterSheet, spriteSize, subsprites);

        if (addToLoaded) 
        {
            loadedFonts.Add(loadedFontDictionary);
            loadedFontResources.Add(characterSheet);
        }

        return loadedFontDictionary;
    }

    public static Dictionary<char, CharData> ReloadFontResource(Texture2D characterSheet) 
    {
        if (IsFontLoaded(characterSheet))
        {
            Dictionary<char, CharData> loadedFontResource = LoadFontResource(characterSheet, false);
            loadedFonts[loadedFontResources.IndexOf(characterSheet)] = loadedFontResource;
            return loadedFontResource;
        }
        else 
        {
            Debug.Log("Font in Texture2D: " + characterSheet.name + " hasn't previously been loaded, Loading normally. Please use LoadFontResource/LoadFontResources if this behaviour is not desired.");
            return LoadFontResource(characterSheet);
        }
    }

    public static bool IsFontLoaded(Texture2D characterSheet) 
    {
        return loadedFontResources.Contains(characterSheet);
    }

    private static Dictionary<char, CharData> GenerateCharFontDictionary(Texture2D characterSheet, int spriteSize, Sprite[] characterSprites)
    {
        //int height = charSheet.height; // We might need this if we ever use a text image that is on more than one line
        int width = characterSheet.width;

        int charIndex = 0;

        Dictionary<char, CharData> charData = new Dictionary<char, CharData>();

        int minY = 0;
        int maxY = minY + spriteSize;

        //Apparently GetPixel is weird and broken and when I use a private method to neaten things up unity just dies
        //if(charIndex >= chars.Length)
        for (int texCoordX = 0; texCoordX < width && charIndex < chars.Length; texCoordX += spriteSize)
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
                    edgeFound = characterSheet.GetPixel(currentX, currentY).a != 0;
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
                    edgeFound = characterSheet.GetPixel(currentX, currentY).a != 0;
                    if (edgeFound) break;
                }
                if (edgeFound) break;
                leftEdge++;
            }

            //Store current sprite width
            int currentSpriteWidth = spriteSize - (leftEdge + rightEdge);

            charData.Add(chars[charIndex], new CharData(currentSpriteWidth, characterSprites[charIndex]));

            charIndex++;
        }

        return charData;
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