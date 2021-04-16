using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using System;

public abstract class XmlLoader
{
    protected string StreamingPath = Application.streamingAssetsPath;
    protected string FileExtension = "xml";
    protected string FilePath;
    protected string TopLevelNode;

    protected LoadedXmlFile[] LoadAllXmlFiles() 
    {
        //List<Tuple<XmlNode, string>> loadedFiles = new List<Tuple<XmlNode, string>>();
        List<LoadedXmlFile> loadedFiles = new List<LoadedXmlFile>();


        if (string.IsNullOrEmpty(TopLevelNode)) 
        {
            Debug.Log("ERROR: XmlLoader has not been given a valid top level node!");
            return loadedFiles.ToArray();
        }

        string filePath = StreamingPath + (string.IsNullOrEmpty(FilePath) ? "/" : "/" + FilePath + (FilePath[FilePath.Length - 1] == '/' ? "" : "/"));

        string[] files = Directory.GetFiles(filePath, "*." + FileExtension);

        XmlDocument doc;

        foreach (string file in files) 
        {
            if (File.Exists(file)) 
            {
                doc = new XmlDocument();
                
                try
                {
                    doc.Load(file);
                    XmlNode tLN = doc.DocumentElement.SelectSingleNode("/" + TopLevelNode);
                    loadedFiles.Add(new LoadedXmlFile(tLN, Path.GetFileNameWithoutExtension(file)));
                }
                catch (XmlException e)
                {
                    Debug.LogError("ERROR: XmlLoader failed to load the xml at path: " + file + ", full stack trace: " + e);
                }
            }
            else
            {
                Debug.LogError("FILE MISSING: file at path: " + file + " doesn't exist!");
            }
        }

        return loadedFiles.ToArray();
    }
}

public struct LoadedXmlFile 
{
    public string Name;
    public XmlNode Node;

    public LoadedXmlFile(XmlNode node, string name) 
    {
        Node = node;
        Name = name;
    }
}
