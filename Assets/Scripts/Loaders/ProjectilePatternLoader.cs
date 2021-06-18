using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class ProjectilePatternLoader : XmlLoader
{
    public static ProjectilePatternLoader current = new ProjectilePatternLoader(); //I'm not sure if this is the best idea but it does seem to work?
    public ProjectilePatternStage[] PatternStages;
    private ProjectilePatternLoader()
    {
        FilePath = "ProjectilePatterns";
        TopLevelNode = "projectile-patterns";
        PatternStages = LoadAllProjectilePatternStages();
    }

    //Reads all files in the streaming assets projectile pattern directory, extracts stages and returns them in order
    private ProjectilePatternStage[] LoadAllProjectilePatternStages()
    {
        return LoadAllXmlFiles() //Load all the xml files in the projectile-pattern directory
            .SelectMany(pFile => pFile.Node.SelectNodes("stage").Cast<XmlNode>()) //Flatten all file stages into one list of stage nodes
            .Select(xmlStageNode =>
            {
                //For each stage node, get the stage number
                XmlNode stageNumberNode = xmlStageNode.Attributes["no"];

                //Then create a new ProjectilePatternStage
                return new ProjectilePatternStage(
                    GetAllProjectilePatternsFromXmlNodeList(xmlStageNode.SelectNodes("projectile-pattern")), //Convert all the "projectile-pattern" nodes within the stage into a collection of projectile patterns
                    stageNumberNode != null && int.TryParse(stageNumberNode.InnerText, out int stageNumberParsed) ? stageNumberParsed : 0 //Store the stage number, or 0 if there isn't one
                    );
            })
            .OrderBy(x => x.stageNumber) //Order this by the stage numbers, if not needed we can just skip this step
            .ToArray(); //Convert this into an array
    }

    private IEnumerable<ProjectilePattern> GetAllProjectilePatternsFromXmlNodeList(XmlNodeList nodeList)
    {
        //Return a collection of ProjectilePatterns (we can then assign these to a given stage)
        return new List<XmlNode>(nodeList.Cast<XmlNode>()).Select(xmlPatternNode =>
        {
            //Convert all the pattern targets into a list
            ProjectilePatternTarget[] projectilePatternTargets = new List<XmlNode>(xmlPatternNode.ChildNodes.Cast<XmlNode>())
                .Where(x => x.NodeType == XmlNodeType.Element)
                .Select(x => ConvertXmlNodeToPatternTarget(x))
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

    private ProjectilePatternTarget ConvertXmlNodeToPatternTarget(XmlNode xmlNode)
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
                !string.IsNullOrEmpty(innerText) && int.TryParse(innerText, out int result) ? ConvertRelativePlatformNumberToTargetType(result) : ProjectileTargetType.Player,
                timingFlagNode != null && float.TryParse(timingFlagNode.InnerText, out float timingModifier) ? timingModifier : 0f,
                entityFireIndexNode != null && int.TryParse(entityFireIndexNode.InnerText, out int entityFireIndex) ? entityFireIndex : -1,
                isStageItem
                );
        }
    }


    private ProjectileTargetType ConvertRelativePlatformNumberToTargetType(int platformValue)
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
