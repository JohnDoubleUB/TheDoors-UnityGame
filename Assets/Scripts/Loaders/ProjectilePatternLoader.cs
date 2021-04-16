using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;
using System.Linq;

public class ProjectilePatternLoader : XmlLoader
{
    public static ProjectilePatternLoader current = new ProjectilePatternLoader();
    public bool test = true;
    private ProjectilePatternLoader()
    {
        FilePath = "ProjectilePatterns";
        TopLevelNode = "projectile-patterns";
        ProjectilePatternStage[] why = LoadAllProjectilePatternStages();
        Debug.Log("Order : " + string.Join(", ", why.Select(x => x.stageNumber)));

        foreach (ProjectilePatternStage w in why) 
        {
            Debug.Log("Stage " + w.stageNumber + ": " + string.Join(", ", w.patterns.Select(x => x.Name)));
        }
    }

    private ProjectilePatternStage[] LoadAllProjectilePatternStages()
    {

        LoadedXmlFile[] pFiles = LoadAllXmlFiles();

        Debug.Log("Files: " + pFiles.Length);
        var allStages = pFiles.SelectMany(pFile => 
        {
            XmlNodeList xmlStageNodes = pFile.Node.SelectNodes("stage");
            return xmlStageNodes.Cast<XmlNode>();
        });

        foreach (XmlNode t in allStages) 
        {
            Debug.Log(t.Attributes["no"].InnerText);
        }



        List<ProjectilePatternStage> patternStages = new List<ProjectilePatternStage>();

        foreach (LoadedXmlFile pFile in pFiles)
        {
            //Read in the stage nodes
            XmlNodeList xmlStageNodes = pFile.Node.SelectNodes("stage");
            Debug.Log("stage count: " + xmlStageNodes.Count);

            //Do magic
            ProjectilePatternStage[] orderedStagesNew = new List<XmlNode>(xmlStageNodes.Cast<XmlNode>())
                .Select(xmlStageNode =>
                {
                    //For each stage node, get the stage number
                    XmlNode stageNumberNode = xmlStageNode.Attributes["no"];

                    //Then create a new ProjectilePatternStage
                    return new ProjectilePatternStage(
                        GetAllProjectilePatternsFromXmlNodeList(xmlStageNode.SelectNodes("projectile-pattern")), //Convert all the "projectile-pattern" nodes within the stage into a collection of projectile patterns
                        stageNumberNode != null && int.TryParse(stageNumberNode.InnerText, out int stageNumberParsed) ? stageNumberParsed : 0 //Store the stage number
                        );
                })
                //.OrderBy(x => x.stageNumber) //Order by stage number
                .ToArray(); //Convert to array

            patternStages.AddRange(orderedStagesNew); //Idc what files these are from
        }

        //Order by stage number after combining all the files
        return patternStages
            .OrderBy(x=> x.stageNumber)
            .ToArray();
    }

    private IEnumerable<ProjectilePattern> GetAllProjectilePatternsFromXmlNodeList(XmlNodeList nodeList) 
    {
        //Return a collection of ProjectilePatterns (we can then assign these to a given stage)
        return new List<XmlNode>(nodeList.Cast<XmlNode>()).Select(xmlPatternNode =>
        {
            //Convert all the pattern targets into a list
            ProjectilePatternTarget[] projectilePatternTargets = new List<XmlNode>(xmlPatternNode.ChildNodes.Cast<XmlNode>())
                .Where(x => x.NodeType == XmlNodeType.Element)
                .Select(x => ConvertToPatternTarget(x))
                .ToArray();

            //Get the any projectile pattern data
            XmlNode durationFlagNode = xmlPatternNode.Attributes["duration"];
            XmlNode nameFlagNode = xmlPatternNode.Attributes["name"];
            XmlNode defaultTimingFlagNode = xmlPatternNode.Attributes["default-timing"];

            //Create the pattern with these pattern targets
            return new ProjectilePattern(
                    projectilePatternTargets,
                    defaultTimingFlagNode != null && float.TryParse(defaultTimingFlagNode.InnerText, out float defaultTiming) ? defaultTiming : 1f,
                    durationFlagNode != null && float.TryParse(durationFlagNode.InnerText, out float duration) ? duration : 5f,
                    nameFlagNode != null ? nameFlagNode.InnerText : ""
                    );

        });
    }

    private ProjectilePatternTarget ConvertToPatternTarget(XmlNode xmlNode) 
    {
        string nodeName = xmlNode.Name;
        bool isStageItem = nodeName == "player-item" || nodeName == "random-item" || nodeName == "target-item";

        XmlNode timingFlagNode = xmlNode.Attributes["timing"];
        XmlNode entityFireIndexNode = xmlNode.Attributes["fire-index"];

        //if its a numbered platform target
        if (nodeName == "target-item" || nodeName == "target")
        {
            return new ProjectilePatternTarget(
                !string.IsNullOrEmpty(xmlNode.InnerText) && int.TryParse(xmlNode.InnerText, out int targetIndex) ? targetIndex : 0, //Index value
                timingFlagNode != null && float.TryParse(timingFlagNode.InnerText, out float timingModifier) ? timingModifier : 0f, //TimingModifier
                entityFireIndexNode != null && int.TryParse(entityFireIndexNode.InnerText, out int entityFireIndex) ? entityFireIndex : -1, //If we care about what entity fires it this will be positive
                isStageItem
                );
        }
        else if (nodeName == "random-item" || nodeName == "random")
        {
            return new ProjectilePatternTarget(
                ProjectileTargetType.Random,
                timingFlagNode != null && float.TryParse(timingFlagNode.InnerText, out float timingModifier) ? timingModifier : 0f,
                entityFireIndexNode != null && int.TryParse(entityFireIndexNode.InnerText, out int entityFireIndex) ? entityFireIndex : -1,
                isStageItem
                );
        }
        else //"player-item" || "player"
        {
            string innerText = xmlNode.InnerText;

            return new ProjectilePatternTarget(
                !string.IsNullOrEmpty(innerText) && int.TryParse(innerText, out int result) ? ConvertRelativePlatformToTargetType(result) : ProjectileTargetType.Player,
                timingFlagNode != null && float.TryParse(timingFlagNode.InnerText, out float timingModifier) ? timingModifier : 0f,
                entityFireIndexNode != null && int.TryParse(entityFireIndexNode.InnerText, out int entityFireIndex) ? entityFireIndex : -1,
                isStageItem
                );
        }
    }


    private ProjectileTargetType ConvertRelativePlatformToTargetType(int platformValue) 
    {
        switch (platformValue)
        {
            case 1:
                return ProjectileTargetType.PlayerPlatform;

            case 2:
                return ProjectileTargetType.PlayerClosestAdjacentPlatform1;

            case 3:
                return ProjectileTargetType.PlayerClosestAdjacentPlatform2;

            case 4:
                return ProjectileTargetType.PlayerClosestAdjacentPlatform3;

            case 5:
                return ProjectileTargetType.PlayerClosestAdjacentPlatform4;

            default:
                return ProjectileTargetType.Player;
        }
    }
}
