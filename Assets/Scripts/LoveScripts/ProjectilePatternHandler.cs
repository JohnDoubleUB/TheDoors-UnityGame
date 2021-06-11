using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectilePatternHandler
{
    private List<Transform> platformTargets;
    private Transform playerTarget; 
    private ProjectilePattern currentPattern;
    private ProjectilePatternTarget currentProjectile;
    private ProjectilePatternStage[] projectilePatternStages;

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
            if (platformTargets.Any())
            {
                return platformTargets.OrderBy(x => Vector3.Distance(x.position, playerTarget.position)).FirstOrDefault();
            }
            else
            {
                return playerTarget;
            }
        }
    }

    public ProjectilePatternStage[] ProjectilePatternStages { get { return projectilePatternStages; } }
    public List<Transform> PlatformTargets { get { return platformTargets; } }
    public Transform PlayerTarget { get { return playerTarget; } }

    public int CurrentPatternIndex { get { return patternIndex; } }
    public int CurrentProjectileIndex { get { return projectileIndex; } }

    public ProjectilePatternHandler(IEnumerable<Transform> platformTargets, Transform playerTarget, params ProjectilePatternStage[] projectilePatternStages)
    {
        this.platformTargets = platformTargets.ToList();
        this.playerTarget = playerTarget;

        if (projectilePatternStages.Any()) 
        {
            this.projectilePatternStages = projectilePatternStages;
            UpdateCurrent();
        }
    }

    public Transform GetCurrentProjectileTargetTransform(bool includeItems = false)
    {
        if (!includeItems && currentProjectile.IsStageItem) NextProjectileTarget(includeItems);
        
        return GetProjectilePatternTargetTransform(currentProjectile);
    }

    public void NextProjectileTarget(bool includeItems = false) 
    {
        do
        {
            currentPattern = projectilePatternStages[stageIndex].patterns[patternIndex];
            projectileIndex = projectileIndex + 1 < currentPattern.PatternTargets.Length ? projectileIndex + 1 : 0;
            currentProjectile = currentPattern.PatternTargets[projectileIndex];
            currentDelay = currentProjectile.TimingModifier <= 0 ? currentPattern.DefaultTiming : currentProjectile.TimingModifier;
        } 
        while (!includeItems && currentProjectile.IsStageItem);
    }

    public Transform GetNextProjectileTargetTransform()
    {
        NextProjectileTarget();
        return GetProjectilePatternTargetTransform(currentProjectile);
    }

    private void UpdateCurrent() 
    {
        currentPattern = projectilePatternStages[stageIndex].patterns[patternIndex];
        currentProjectile = currentPattern.PatternTargets[projectileIndex];
        currentDelay = currentProjectile.TimingModifier <= 0 ? currentPattern.DefaultTiming : currentProjectile.TimingModifier;
    }

    private Transform GetProjectilePatternTargetTransform(ProjectilePatternTarget projectilePatternTarget)
    {
        switch (projectilePatternTarget.TargetType)
        {
            case ProjectileTargetType.Target:
                if (projectilePatternTarget.TargetIndex < platformTargets.Count) return platformTargets[projectilePatternTarget.TargetIndex];
                break;

            case ProjectileTargetType.Random:
                return platformTargets[Random.Range(0, platformTargets.Count)];

            case ProjectileTargetType.PlayerPlatform:
                //Get index of closest platform point
                int playerPointPositionIndex = platformTargets.IndexOf(ClosestPlatformPointToPlayer);
                int nextPosition = playerPointPositionIndex % 2 == 0 ? playerPointPositionIndex + 1 : playerPointPositionIndex - 1;
                return platformTargets[nextPosition];

            case ProjectileTargetType.PlayerClosestAdjacentPlatform1:
                playerPointPositionIndex = platformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if we're on the left or right of a platform
                if (playerPointPositionIndex % 2 == 0)
                {
                    return playerPointPositionIndex == 0 ? platformTargets[playerPointPositionIndex + 2] : platformTargets[playerPointPositionIndex - 1];
                }
                else
                {
                    return playerPointPositionIndex == platformTargets.Count - 1 ? platformTargets[playerPointPositionIndex - 2] : platformTargets[playerPointPositionIndex + 1];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform2:
                //Check if we're on the left or right of a platform
                playerPointPositionIndex = platformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if we're on the left or right of a platform
                if (playerPointPositionIndex % 2 == 0)
                {
                    return playerPointPositionIndex == 0 ? platformTargets[playerPointPositionIndex + 3] : platformTargets[playerPointPositionIndex - 2];
                }
                else
                {
                    return playerPointPositionIndex == platformTargets.Count - 1 ? platformTargets[playerPointPositionIndex - 3] : platformTargets[playerPointPositionIndex + 2];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform3:
                playerPointPositionIndex = platformTargets.IndexOf(ClosestPlatformPointToPlayer);

                //Check if the player is on the left or right side
                if (playerPointPositionIndex < platformTargets.Count / 2)
                {
                    return platformTargets[platformTargets.Count - 2];
                }
                else
                {
                    return platformTargets[1];
                }

            case ProjectileTargetType.PlayerClosestAdjacentPlatform4:
                playerPointPositionIndex = platformTargets.IndexOf(ClosestPlatformPointToPlayer);

                if (playerPointPositionIndex < platformTargets.Count / 2)
                {
                    return platformTargets[platformTargets.Count - 1];
                }
                else
                {
                    return platformTargets[0];
                }

        }

        return ClosestPlatformPointToPlayer;
    }

    public bool SetPatternIndex(int patternIndex) //Returns true if value is set to the provided pattern index and false if the index has looped either from last index to first or first to last
    {
        bool patternIndexIsValid = patternIndex >= 0 && patternIndex < projectilePatternStages[stageIndex].patterns.Length;

        this.patternIndex = patternIndexIsValid ? patternIndex : 0;
        projectileIndex = 0;


        UpdateCurrent();
        return patternIndexIsValid;
    }

    public bool SetToNextPattern() // return boolean true if pattern is infact next pattern (i.e. we haven't looped back to the first pattern)
    {
        return SetPatternIndex(patternIndex + 1);
    }

    public bool SetPreviousToPattern()
    {
        return SetPatternIndex(patternIndex - 1);
    }

    public void SetStage(int stageIndex) 
    {

        this.stageIndex = stageIndex >= 0 && stageIndex < projectilePatternStages.Length ? stageIndex : 0;
        //The stage has changed! therefore we want to reset both the projectile index and the pattern index to 0!
        projectileIndex = 0;
        patternIndex = 0;
        UpdateCurrent();
    }

    public void SetToPreviousStage() 
    {
        SetStage(stageIndex - 1);
    }

    public void SetToNextStage() 
    {
        SetStage(stageIndex + 1);
    }
}
