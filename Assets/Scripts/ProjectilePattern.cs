﻿using System.Collections;
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

public class ProjectilePatternHandler
{
    public ProjectilePattern[] ProjectilePatterns;
    public List<Transform> PlatformTargets;
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

    private Transform ClosestPlatformPointToPlayer 
    {
        get
        {
            if (PlatformTargets.Any())
            {
                return PlatformTargets.OrderBy(x => Vector3.Distance(x.position, PlayerTarget.position)).FirstOrDefault();
            }
            else 
            {
                return PlayerTarget;
            }
        }
    }

    public int CurrentPatternIndex { get { return patternIndex; } }
    public int CurrentProjectileIndex { get { return projectileIndex; } }

    public ProjectilePatternHandler(IEnumerable<Transform> platformTargets, Transform playerTarget, params ProjectilePattern[] projectilePatterns)
    {
        PlatformTargets = platformTargets.ToList();
        PlayerTarget = playerTarget;
        
        if (projectilePatterns.Any()) 
        {
            ProjectilePatterns = projectilePatterns;
            UpdateCurrent();
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

    private void UpdateCurrent() 
    {
        currentPattern = ProjectilePatterns[patternIndex];
        currentProjectile = currentPattern.PatternTargets[projectileIndex];
        currentDelay = currentProjectile.TimingModifier <= 0 ? currentPattern.DefaultTiming : currentProjectile.TimingModifier;
    }

    private Transform GetProjectilePatternTargetTransform(ProjectilePatternTarget projectilePatternTarget) 
    {
        switch (projectilePatternTarget.TargetType) 
        {
            case ProjectileTargetType.Target:
                if (projectilePatternTarget.TargetIndex < PlatformTargets.Count) return PlatformTargets[projectilePatternTarget.TargetIndex];
                break;

            case ProjectileTargetType.Random:
                return PlatformTargets[Random.Range(0, PlatformTargets.Count)];

            case ProjectileTargetType.PlayerPlatform:
                //Get index of closest platform point
                int playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);
                int nextPosition = playerPointPositionIndex % 2 == 0 ? playerPointPositionIndex + 1 : playerPointPositionIndex - 1;
                return PlatformTargets[nextPosition];

            case ProjectileTargetType.PlayerClosestAdjacentPlatform1:
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if we're on the left or right of a platform
                if (playerPointPositionIndex % 2 == 0)
                {
                    return playerPointPositionIndex == 0 ? PlatformTargets[playerPointPositionIndex + 2] : PlatformTargets[playerPointPositionIndex - 1];
                }
                else 
                {
                    return playerPointPositionIndex == PlatformTargets.Count - 1 ? PlatformTargets[playerPointPositionIndex - 2] : PlatformTargets[playerPointPositionIndex + 1];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform2:
                //Check if we're on the left or right of a platform
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if we're on the left or right of a platform
                if (playerPointPositionIndex % 2 == 0)
                {
                    return playerPointPositionIndex == 0 ? PlatformTargets[playerPointPositionIndex + 3] : PlatformTargets[playerPointPositionIndex - 2];
                }
                else
                {
                    return playerPointPositionIndex == PlatformTargets.Count - 1 ? PlatformTargets[playerPointPositionIndex - 3] : PlatformTargets[playerPointPositionIndex + 2];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform3:
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if the player is on the left or right side
                if (playerPointPositionIndex < PlatformTargets.Count / 2)
                {
                    return PlatformTargets[PlatformTargets.Count - 2];
                }
                else 
                {
                    return PlatformTargets[1];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform4:
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);
                
                if (playerPointPositionIndex < PlatformTargets.Count / 2)
                {
                    return PlatformTargets[PlatformTargets.Count - 1];
                }
                else
                {
                    return PlatformTargets[0];
                }

        }

        return ClosestPlatformPointToPlayer;
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

        UpdateCurrent();
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

public class ProjectilePatternHandler2
{
    public ProjectilePattern[] ProjectilePatterns; 
    public List<Transform> PlatformTargets;
    public Transform PlayerTarget;

    private ProjectilePattern currentPattern;
    private ProjectilePatternTarget currentProjectile;

    private int patternIndex = 0;
    private int stageIndex = 0;
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

    private Transform ClosestPlatformPointToPlayer
    {
        get
        {
            if (PlatformTargets.Any())
            {
                return PlatformTargets.OrderBy(x => Vector3.Distance(x.position, PlayerTarget.position)).FirstOrDefault();
            }
            else
            {
                return PlayerTarget;
            }
        }
    }

    public int CurrentPatternIndex { get { return patternIndex; } }
    public int CurrentProjectileIndex { get { return projectileIndex; } }

    public ProjectilePatternHandler2(IEnumerable<Transform> platformTargets, Transform playerTarget, params ProjectilePattern[] projectilePatterns)
    {
        PlatformTargets = platformTargets.ToList();
        PlayerTarget = playerTarget;

        if (projectilePatterns.Any())
        {
            ProjectilePatterns = projectilePatterns;
            UpdateCurrent();
            Debug.Log("current delay = " + currentDelay + " current pattern default timeing " + currentPattern.DefaultTiming + " current projectile " + currentProjectile.TimingModifier);
        }
    }

    public ProjectilePatternHandler2(IEnumerable<Transform> platformTargets, Transform playerTarget, params ProjectilePatternStage[] projectilePatterStages) 
    {

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

    private void UpdateCurrent()
    {
        currentPattern = ProjectilePatterns[patternIndex];
        currentProjectile = currentPattern.PatternTargets[projectileIndex];
        currentDelay = currentProjectile.TimingModifier <= 0 ? currentPattern.DefaultTiming : currentProjectile.TimingModifier;
    }

    private Transform GetProjectilePatternTargetTransform(ProjectilePatternTarget projectilePatternTarget)
    {
        switch (projectilePatternTarget.TargetType)
        {
            case ProjectileTargetType.Target:
                if (projectilePatternTarget.TargetIndex < PlatformTargets.Count) return PlatformTargets[projectilePatternTarget.TargetIndex];
                break;

            case ProjectileTargetType.Random:
                return PlatformTargets[Random.Range(0, PlatformTargets.Count)];

            case ProjectileTargetType.PlayerPlatform:
                //Get index of closest platform point
                int playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);
                int nextPosition = playerPointPositionIndex % 2 == 0 ? playerPointPositionIndex + 1 : playerPointPositionIndex - 1;
                return PlatformTargets[nextPosition];

            case ProjectileTargetType.PlayerClosestAdjacentPlatform1:
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if we're on the left or right of a platform
                if (playerPointPositionIndex % 2 == 0)
                {
                    return playerPointPositionIndex == 0 ? PlatformTargets[playerPointPositionIndex + 2] : PlatformTargets[playerPointPositionIndex - 1];
                }
                else
                {
                    return playerPointPositionIndex == PlatformTargets.Count - 1 ? PlatformTargets[playerPointPositionIndex - 2] : PlatformTargets[playerPointPositionIndex + 1];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform2:
                //Check if we're on the left or right of a platform
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if we're on the left or right of a platform
                if (playerPointPositionIndex % 2 == 0)
                {
                    return playerPointPositionIndex == 0 ? PlatformTargets[playerPointPositionIndex + 3] : PlatformTargets[playerPointPositionIndex - 2];
                }
                else
                {
                    return playerPointPositionIndex == PlatformTargets.Count - 1 ? PlatformTargets[playerPointPositionIndex - 3] : PlatformTargets[playerPointPositionIndex + 2];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform3:
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if the player is on the left or right side
                if (playerPointPositionIndex < PlatformTargets.Count / 2)
                {
                    return PlatformTargets[PlatformTargets.Count - 2];
                }
                else
                {
                    return PlatformTargets[1];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform4:
                playerPointPositionIndex = PlatformTargets.IndexOf(ClosestPlatformPointToPlayer);

                if (playerPointPositionIndex < PlatformTargets.Count / 2)
                {
                    return PlatformTargets[PlatformTargets.Count - 1];
                }
                else
                {
                    return PlatformTargets[0];
                }

        }

        return ClosestPlatformPointToPlayer;
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

        UpdateCurrent();
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
    private int fireIndex = -1; //This determines preference for what robot is firing these, if its -1 then we don't care
    private bool isStageItem;

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