using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilePattern
{
    public float DefaultTiming = 1f;
    public ProjectilePatternTarget[] PatternTargets;

    public ProjectilePattern(params ProjectilePatternTarget[] patternTargets) 
    {
        PatternTargets = patternTargets;
    }
}

public class ProjectilePatternHandler
{
    public ProjectilePattern[] ProjectilePatterns;
    public Transform[] PlatformTargets;
    public Transform PlayerTarget;
    
    private ProjectilePattern currentPattern;
    private ProjectilePatternTarget currentProjectile;

    private int patternIndex = 0;
    private int projectileIndex = 0;
    private float currentDelay;

    public ProjectilePatternTarget CurrentProjectile 
    {
        get { return currentProjectile; } 
    }

    public ProjectilePattern CurrentPattern 
    {
        get { return currentPattern; }
    }

    public float CurrentDelay 
    {
        get { return currentDelay; }
    }

    public int CurrentPatternIndex { get { return patternIndex; } }
    public int CurrentProjectileIndex { get { return projectileIndex; } }

    public ProjectilePatternHandler(IEnumerable<Transform> platformTargets, Transform playerTarget, params ProjectilePattern[] projectilePatterns)
    {
        PlatformTargets = platformTargets.ToArray();
        PlayerTarget = playerTarget;
        
        if (projectilePatterns.Any()) 
        {
            ProjectilePatterns = projectilePatterns;
            currentPattern = ProjectilePatterns[patternIndex];
            currentProjectile = currentPattern.PatternTargets[projectileIndex];
            currentDelay = currentProjectile.TimingModifier <= 0 ? currentPattern.DefaultTiming : currentProjectile.TimingModifier;
            Debug.Log("current delay = " + currentDelay + " current pattern default timeing " + currentPattern.DefaultTiming + " current projectile " + currentProjectile.TimingModifier);
        }
    }

    public Transform GetCurrentProjectileTargetTransform() 
    {
        return GetProjectilePatternTargetTransform(currentProjectile);
    }

    public void NextProjectileTarget() 
    {
        currentPattern = ProjectilePatterns[patternIndex];
        projectileIndex = projectileIndex + 1 < currentPattern.PatternTargets.Length ? projectileIndex + 1 : 0;
        currentProjectile = currentPattern.PatternTargets[projectileIndex];
        currentDelay = currentProjectile.TimingModifier <= 0 ? currentPattern.DefaultTiming : currentProjectile.TimingModifier;
    }

    public Transform GetNextProjectileTargetTransform()
    {
        NextProjectileTarget();
        return GetProjectilePatternTargetTransform(currentProjectile);
    }

    private Transform GetProjectilePatternTargetTransform(ProjectilePatternTarget projectilePatternTarget) 
    {
        switch (projectilePatternTarget.TargetType) 
        {
            case ProjectileTargetType.Target:
                if (projectilePatternTarget.TargetIndex < PlatformTargets.Length) return PlatformTargets[projectilePatternTarget.TargetIndex];
                break;

            case ProjectileTargetType.Random:
                return PlatformTargets[Random.Range(0, PlatformTargets.Length)];
        }

        return PlayerTarget;
    }

    public void SetPatternIndex(int patternIndex) 
    {
        if (patternIndex >= 0 && patternIndex < ProjectilePatterns.Length)
        {
            this.patternIndex = patternIndex;
        }
        else 
        {
            this.patternIndex = 0;
        }

        projectileIndex = 0;
    }

    public void SetNextPatternIndex() 
    {
        SetPatternIndex(patternIndex + 1);
    }

    public void SetLastPatternNext() 
    {
        SetPatternIndex(patternIndex - 1);
    }
}

public class ProjectilePatternTarget
{
    private float timingModifier = -1;
    private int targetIndex;
    private ProjectileTargetType targetType = ProjectileTargetType.Player;

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

    public ProjectilePatternTarget(float timingModifier = 0f) 
    {
        this.timingModifier = timingModifier;
    }

    public ProjectilePatternTarget(int targetIndex, float timingModifier = 0f) 
    {
        this.targetIndex = targetIndex;
        targetType = ProjectileTargetType.Target;
        this.timingModifier = timingModifier;
    }

    public ProjectilePatternTarget(ProjectileTargetType targetType, float timingModifier = 0f) 
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
    Target
}