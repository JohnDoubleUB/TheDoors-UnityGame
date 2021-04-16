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
        LoadAllProjectilePatterns();
    }

    private ProjectilePattern[] LoadAllProjectilePatterns()
    {

        LoadedXmlFile[] pFiles = LoadAllXmlFiles();
        List<ProjectilePattern> projectilePatterns = new List<ProjectilePattern>();
        ProjectilePatternTarget[] projectilePatternTargets;

        foreach (LoadedXmlFile pFile in pFiles)
        {
            //Read in the stage nodes
            XmlNodeList xmlStageNodes = pFile.Node.SelectNodes("stage");


            IEnumerable<XmlNode> orderedStages = new List<XmlNode>(xmlStageNodes.Cast<XmlNode>())
                .OrderBy(xmlStageNode => 
                {
                    XmlNode stageNumberNode = xmlStageNode.Attributes["no"];
                    return stageNumberNode != null && int.TryParse(stageNumberNode.InnerText, out int stageNumberParsed) ? stageNumberParsed : 0;
                });




            //Grab the projectile nodes
            XmlNodeList xmlPatternNodes = pFile.Node.SelectNodes("projectile-pattern");

            foreach (XmlNode xmlPatternNode in xmlPatternNodes)
            {
                projectilePatternTargets = new List<XmlNode>(xmlPatternNode.ChildNodes.Cast<XmlNode>())
                    .Where(x => x.NodeType == XmlNodeType.Element)
                    .Select(x => ConvertToPatternTarget(x))
                    .ToArray();

                //Get duration, name and default timing
                XmlNode durationFlagNode = xmlPatternNode.Attributes["duration"];
                XmlNode nameFlagNode = xmlPatternNode.Attributes["name"];
                XmlNode defaultTimingFlagNode = xmlPatternNode.Attributes["default-timing"];

                projectilePatterns.Add(
                    new ProjectilePattern(
                        projectilePatternTargets,
                        defaultTimingFlagNode != null && float.TryParse(defaultTimingFlagNode.InnerText, out float defaultTiming) ? defaultTiming : 1f,
                        durationFlagNode != null && float.TryParse(durationFlagNode.InnerText, out float duration) ? duration : 5f,
                        nameFlagNode != null ? nameFlagNode.InnerText : ""
                    ));
            }
        }

        return projectilePatterns.ToArray();
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
