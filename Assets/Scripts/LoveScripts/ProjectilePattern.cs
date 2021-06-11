using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilePattern
{
    public float DefaultTiming = 1f; //This is default timing between a given pattern target
    public ProjectilePatternTarget[] PatternTargets;
    public float PatternDuration = 5f; //This is timing for each pattern, default is 5 seconds
    public string Name;
    public ProjectilePattern(params ProjectilePatternTarget[] patternTargets) 
    {
        PatternTargets = patternTargets;
    }

    public ProjectilePattern SetDefaultTiming(float defaultTiming) 
    {
        DefaultTiming = defaultTiming;
        return this;
    }

    public ProjectilePattern SetDefaultDuration(float patternDuration) 
    {
        PatternDuration = patternDuration;
        return this;
    }

    public ProjectilePattern(ProjectilePatternTarget[] patternTargets, float defaultTiming, float patternDuration, string name) 
    {
        PatternTargets = patternTargets;
        DefaultTiming = defaultTiming;
        PatternDuration = patternDuration;
        Name = name;
    }
}

public class ProjectilePatternStage 
{
    public ProjectilePattern[] patterns;
    public int stageNumber;
    public ProjectilePatternStage(IEnumerable<ProjectilePattern> patterns, int stageNumber = 0) 
    {
        this.patterns = patterns.ToArray();
        this.stageNumber = stageNumber;
    }
}


public class ProjectilePatternTarget
{
    private float timingModifier = -1; //This is the timing modified for a particular pattern, it defaults to -1, I think it usually gets set to 0 though idk
    private int targetIndex; //This is used to define the target when we are chosing an absolute target platform for a shot
    private ProjectileTargetType targetType = ProjectileTargetType.Player; //This determines the target, some targets are based around player or are random besides that there is also a specified target platform option
    private int fireIndex = -1; //This determines preference for what robot is firing these, if its -1 then we don't care
    private bool isStageItem; //This defines if a projectile is a stage item, if it is then we might care about that

    public ProjectileTargetType TargetType
    {
        get { return targetType; }
    }

    public int TargetIndex 
    {
        get { return targetIndex; }
    }

    public float TimingModifier 
    { 
        get { return timingModifier; } 
    }

    public bool HasFirerPreference 
    {
        get { return fireIndex != -1; }
    }

    public int FireIndex 
    {
        get { return fireIndex; }
    }

    public bool IsStageItem 
    {
        get { return isStageItem; }
    }

    public ProjectilePatternTarget(float timingModifier = 0f, int entityFireIndex = -1) 
    {
        this.timingModifier = timingModifier;
        this.fireIndex = entityFireIndex;
    }

    public ProjectilePatternTarget(int targetIndex, float timingModifier = 0f, int entityFireIndex = -1, bool isStageItem = false) 
    {
        this.targetIndex = targetIndex;
        targetType = ProjectileTargetType.Target;
        this.timingModifier = timingModifier;
        this.fireIndex = entityFireIndex;
        this.isStageItem = isStageItem;
    }

    public ProjectilePatternTarget(ProjectileTargetType targetType, float timingModifier = 0f, int entityFireIndex = -1, bool isStageItem = false) 
    {
        if (targetType != ProjectileTargetType.Target)
        {
            this.targetType = targetType;
        }
        else
        {
            this.targetType = ProjectileTargetType.Player;
        }

        this.timingModifier = timingModifier;
        this.fireIndex = entityFireIndex;
        this.isStageItem = isStageItem;
    }

    public ProjectilePatternTarget(ProjectileTargetType targetType, int targetIndex, float timingModifier = 0f, int entityFireIndex = -1) 
    {
        this.timingModifier = timingModifier;
        this.targetType = targetType;
        this.targetIndex = targetIndex;
        this.fireIndex = entityFireIndex;
    }

    //This should make things a little easier idk
    public static implicit operator ProjectilePatternTarget(int index) 
    {
        return new ProjectilePatternTarget(index);
    }

    public static implicit operator ProjectilePatternTarget(ProjectileTargetType targetType)
    {
        return new ProjectilePatternTarget(targetType);
    }

}

public enum ProjectileTargetType 
{
    Player,
    Random,
    Target,
    PlayerPlatform,
    PlayerClosestAdjacentPlatform1,
    PlayerClosestAdjacentPlatform2,
    PlayerClosestAdjacentPlatform3,
    PlayerClosestAdjacentPlatform4
}