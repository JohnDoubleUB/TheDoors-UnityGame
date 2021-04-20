﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoveLevelManager : MonoBehaviour
{
    public static LoveLevelManager current;

    public Transform[] loveRobotSpawnLocations;
    public Transform[] loveRobotMovementPoints;
    public LovePlatform[] platforms;
    public ParticleSystem heartSmashParticleEffect;

    public LightController2D[] globalLights;

    public NPCDataObject loveNpcData;
    private int npcConvoCount = 0;

    //Hide in inspector?
    public List<Transform> platformPoints = new List<Transform>();

    public float shotSpeed = 0.5f;
    private float currentShotTime = 0f;
    private float lightDelayMultiplier = 1f;
    //public Transform[] playerMovementPoints

    public LoveRobot loveRobotPrefab;

    public List<LoveRobot> loveRobots = new List<LoveRobot>();

    private bool phaseCompleted = true;
    private bool stageCompleted = true;
    private bool phaseTransition = false;
    //private bool alternator = false;
    private bool allCandlesOn = false;

    public int stage = 0;
    public int phase = 0;
    public int playerPointPositionIndex;

    private float testIndexCounter = 0;
    private ProjectilePatternHandler testPatternManager;
    private ProjectilePatternHandlerV2 testPatternManagerV2;
    private bool isFiringPhase;

    public bool fastIntro = true;

    private void Awake()
    {
        if (current == null) current = this;
        if (platforms != null && platforms.Any()) platformPoints = platforms.Select(x => x.LeftAndRight).SelectMany(x => x).ToList();
    }
    void Start()
    {

        //New pattern manager thing
        
        //First patterns
        ProjectilePattern playerPattern1 = new ProjectilePattern(ProjectileTargetType.Player).SetDefaultTiming(0.7f); // 10 seconds
        ProjectilePattern playerPattern2 = new ProjectilePattern(ProjectileTargetType.Player); // 5 seconds
        ProjectilePattern playerPattern3 = new ProjectilePattern(ProjectileTargetType.Player).SetDefaultTiming(2); // 5 seconds
        // playerPattern1, playerPattern2, playerPattern3,

        //ProjectilePatternStage stage1 = new ProjectilePatternStage(playerPattern1, playerPattern2, playerPattern3);

        //Second patterns
        ProjectilePattern playerAnd2RandomPattern1 = new ProjectilePattern(new ProjectilePatternTarget(ProjectileTargetType.Player, 0.5f), ProjectileTargetType.Random, ProjectileTargetType.Random).SetDefaultTiming(10); // 10 seconds
        ProjectilePattern playerAnd2RandomPattern2 = new ProjectilePattern(
            new ProjectilePatternTarget(ProjectileTargetType.Player, 0.5f),
            ProjectileTargetType.Random, 
            ProjectileTargetType.Random, 
            new ProjectilePatternTarget(ProjectileTargetType.Player, 2f), 
            ProjectileTargetType.Random)
            .SetDefaultTiming(10); // 5 seconds
        ProjectilePattern playerAnd2RandomPattern3 = 
            new ProjectilePattern(
            new ProjectilePatternTarget(ProjectileTargetType.Player, 1f), 
            ProjectileTargetType.Random, 
            ProjectileTargetType.Random, 
            new ProjectilePatternTarget(ProjectileTargetType.Player, 2f), 
            ProjectileTargetType.Random)
            .SetDefaultTiming(10); // 7 seconds
        //Third patterns
        ProjectilePattern wavePattern1 = new ProjectilePattern(0, 1, 2, 3, 4, 5, new ProjectilePatternTarget(4, 2f), 3, 2, 1).SetDefaultTiming(2); // 10
        ProjectilePattern wavePattern2 = new ProjectilePattern(0, 1, 2, 3, 4, 5, new ProjectilePatternTarget(4, 1.7f), 3, 2, 1).SetDefaultTiming(3); // 7
        ProjectilePattern wavePattern3 = new ProjectilePattern(0, 1, 2, 3, 4, 5, new ProjectilePatternTarget(4, 1.5f), 3, 2, 1, ProjectileTargetType.Player).SetDefaultTiming(3); // 7

        //Fourth patterns
        ProjectilePattern sprayPattern1 = new ProjectilePattern(
            0, new ProjectilePatternTarget(5, 100),
            1, new ProjectilePatternTarget(4, 100),
            2, new ProjectilePatternTarget(3, 100),
            1, new ProjectilePatternTarget(4, 100),
            0, new ProjectilePatternTarget(5, 100)
            ).SetDefaultTiming(1.7f); // 8 seconds
        
        ProjectilePattern sprayPattern2 = new ProjectilePattern(
            new ProjectilePatternTarget(0, 1f), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10),
            1, new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10),
            2, new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10),
            3, new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10),
            4, new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10),
            new ProjectilePatternTarget(5, 1f), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10),
            4, new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10),
            3, new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10),
            2, new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10),
            1, new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10)
            ).SetDefaultTiming(5); // 8 seconds

        ProjectilePattern sprayPattern3 = new ProjectilePattern(
            new ProjectilePatternTarget(0, 1f), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10),
            0, new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10),
            0, new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10),  
            0, new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10),
            1, new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10),
            
            new ProjectilePatternTarget(5, 1f), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10),
            
            5, new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10),
            
            5, new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10),
            
            5, new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10),
            4, new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10)
            ).SetDefaultTiming(8); // 8 seconds


        //Here
        ProjectilePattern sprayPattern4 = new ProjectilePattern(
            new ProjectilePatternTarget(0, 1f),
            new ProjectilePatternTarget(5, 100), new ProjectilePatternTarget(4, 100), new ProjectilePatternTarget(3, 100),

            new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            0, new ProjectilePatternTarget(5, 100), new ProjectilePatternTarget(4, 100),
            
            new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            0, new ProjectilePatternTarget(5, 100),
            
            new ProjectilePatternTarget(0, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            0, new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            1, new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10),
            
            new ProjectilePatternTarget(5, 1f),
            
            new ProjectilePatternTarget(0, 100), new ProjectilePatternTarget(1, 100), new ProjectilePatternTarget(2, 100),

            new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            5, new ProjectilePatternTarget(0, 100), new ProjectilePatternTarget(1, 100),

            new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            5, new ProjectilePatternTarget(0, 100),

            new ProjectilePatternTarget(5, 10), new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            5, new ProjectilePatternTarget(4, 10), new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(ProjectileTargetType.Player, 100),
            
            4, new ProjectilePatternTarget(3, 10), new ProjectilePatternTarget(2, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10), new ProjectilePatternTarget(1, 10)
            ).SetDefaultTiming(15); // 8 seconds

        //Fifth patterns
        ProjectilePattern playerFocus1 = new ProjectilePattern(
            new ProjectilePatternTarget(ProjectileTargetType.Player, 1f),
            ProjectileTargetType.PlayerPlatform,
            ProjectileTargetType.PlayerClosestAdjacentPlatform2,
            ProjectileTargetType.PlayerClosestAdjacentPlatform3,
            ProjectileTargetType.PlayerClosestAdjacentPlatform4,
            new ProjectilePatternTarget(ProjectileTargetType.Player, 1f),
            ProjectileTargetType.PlayerClosestAdjacentPlatform1,
            ProjectileTargetType.PlayerClosestAdjacentPlatform2,
            ProjectileTargetType.PlayerClosestAdjacentPlatform3,
            ProjectileTargetType.PlayerClosestAdjacentPlatform4
            ).SetDefaultTiming(5f); // 10

        //TODO: Continue to enter these patterns ye
        ProjectilePattern playerFocus2 = new ProjectilePattern(
            new ProjectilePatternTarget(ProjectileTargetType.PlayerClosestAdjacentPlatform1, 0.5f), ProjectileTargetType.PlayerClosestAdjacentPlatform2, ProjectileTargetType.PlayerClosestAdjacentPlatform3, ProjectileTargetType.PlayerClosestAdjacentPlatform4, ProjectileTargetType.PlayerPlatform,
            
            ProjectileTargetType.PlayerClosestAdjacentPlatform1, ProjectileTargetType.PlayerClosestAdjacentPlatform2, ProjectileTargetType.PlayerClosestAdjacentPlatform3, ProjectileTargetType.PlayerClosestAdjacentPlatform4, ProjectileTargetType.PlayerPlatform,
            
            ProjectileTargetType.PlayerClosestAdjacentPlatform1, ProjectileTargetType.PlayerClosestAdjacentPlatform2, ProjectileTargetType.PlayerClosestAdjacentPlatform3, ProjectileTargetType.PlayerClosestAdjacentPlatform4, ProjectileTargetType.PlayerPlatform,
            
            ProjectileTargetType.PlayerClosestAdjacentPlatform1, ProjectileTargetType.PlayerClosestAdjacentPlatform2, ProjectileTargetType.PlayerClosestAdjacentPlatform3, ProjectileTargetType.PlayerClosestAdjacentPlatform4, ProjectileTargetType.PlayerPlatform,
            
            new ProjectilePatternTarget(ProjectileTargetType.Player, 4f), ProjectileTargetType.Player, ProjectileTargetType.Player
            ).SetDefaultTiming(1000); //7

        ProjectilePattern playerFocus3 = new ProjectilePattern(
            new ProjectilePatternTarget(ProjectileTargetType.Player, 1f), ProjectileTargetType.Player, ProjectileTargetType.Player, ProjectileTargetType.Player, ProjectileTargetType.Player,
            new ProjectilePatternTarget(ProjectileTargetType.Player, 3f), ProjectileTargetType.Player, ProjectileTargetType.PlayerPlatform, ProjectileTargetType.PlayerPlatform, ProjectileTargetType.PlayerClosestAdjacentPlatform1, ProjectileTargetType.PlayerClosestAdjacentPlatform1
            ).SetDefaultTiming(20f); //7


        //Final Pattern
        ProjectilePattern finalPattern = new ProjectilePattern(
            ProjectileTargetType.PlayerClosestAdjacentPlatform1,
            ProjectileTargetType.PlayerClosestAdjacentPlatform3,
            ProjectileTargetType.PlayerPlatform,
            ProjectileTargetType.PlayerClosestAdjacentPlatform2,
            ProjectileTargetType.PlayerClosestAdjacentPlatform4,
            ProjectileTargetType.PlayerClosestAdjacentPlatform3,
            ProjectileTargetType.PlayerPlatform,
            ProjectileTargetType.PlayerClosestAdjacentPlatform2
            ).SetDefaultTiming(6f); //7

        //Get a middle platform point
        playerPointPositionIndex = platformPoints.Count / 2;

        //Get the player gameObject
        Player currentPlayer = GameManager.current.player;

        //Set the position of the player https://forum.unity.com/threads/drag-and-drop-streaming-asset-to-inspector-to-get-file-path.499055/
        currentPlayer.gameObject.transform.position = platformPoints[playerPointPositionIndex].position;
        currentPlayer.gameObject.transform.parent = platformPoints[playerPointPositionIndex];

        testPatternManager = new ProjectilePatternHandler(
            platformPoints,
            currentPlayer.transform,
            playerPattern1, playerPattern2, playerPattern3,
            playerAnd2RandomPattern1, playerAnd2RandomPattern2, playerAnd2RandomPattern3,
            wavePattern1, wavePattern2, wavePattern3,
            sprayPattern1, sprayPattern2, sprayPattern3, sprayPattern4,
            playerFocus1, playerFocus2, playerFocus3,
            finalPattern
            );

        testPatternManagerV2 = new ProjectilePatternHandlerV2(platformPoints, currentPlayer.transform, ProjectilePatternLoader.current.PatternStages);
    }

    public Transform MovePlayerToNewPositionPoint(int positionChange)
    {
        int newPosition = playerPointPositionIndex + positionChange;
        if (newPosition < 0) playerPointPositionIndex = 0;
        else if (newPosition >= platformPoints.Count) playerPointPositionIndex = platformPoints.Count - 1;
        else playerPointPositionIndex = newPosition;

        return platformPoints[playerPointPositionIndex];
    }

    public void SpawnParticleEffectAtPosition(Vector3 position)
    {
        if (heartSmashParticleEffect != null)
        {
            heartSmashParticleEffect.gameObject.transform.position = position;
            heartSmashParticleEffect.Play();
        }

        //Hit platform
        LovePlatform closestPlatform = platforms.OrderBy(x => Vector3.Distance(x.transform.position, position)).First();

        if (closestPlatform != null)
        {
            closestPlatform.TakeHit(position);
        }

    }

    private void FireProjectilePatterns() 
    {
        if (testIndexCounter >= 1.0f)
        {
            Transform target = testPatternManager.GetCurrentProjectileTargetTransform();
            FireAtTarget(target.position);
            testPatternManager.NextProjectileTarget();
            testIndexCounter = 0f;
        }
        else
        {
            testIndexCounter += Time.deltaTime * testPatternManager.CurrentDelay;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (!GameManager.current.GameIsOver && !phaseTransition)
        {
            if (stageCompleted)
            {
                stageCompleted = false;
                phaseCompleted = true;
            }

            StartStagePhase();
            
            PhaseUpdate();
        }
    }

    private void StartStagePhase() 
    {
        if (phaseCompleted) 
        {
            print("it happens for stage " + stage);
            switch (stage) 
            {
                case 0:
                    IntroStage();
                    break;
                case 1:
                    LoveRobotEntrance();
                    break;
                case 2:
                    print("this happens");
                    CombatStage_1();
                    break;
            }

            phaseCompleted = false;
            
        }
    }

    //Stage Starts
    private void IntroStage() 
    {
        if (fastIntro && phase < 2) phase = 2;

        switch (phase) 
        {
            case 0:
                //Wait a few seconds
                CompletePhaseAfterSeconds(1f);
                break;
            case 1:
                CompletePhaseAfterSeconds(2f);
                //Turn on lights and wait for that to happen
                break;
            case 2:
                //Turn on main lights
                foreach (LightController2D globalLight in globalLights) globalLight.lightOn = true;
                
                //Fast intro
                if (fastIntro) 
                {
                    foreach (LovePlatform lovePlatform in platforms) 
                    {
                        lovePlatform.leftCandle.lightOn = true;
                        lovePlatform.rightCandle.lightOn = true;
                    }
                }

                CompletePhaseAfterSeconds(4f);
                break;
            case 3:
                if (loveNpcData != null) IntiateLoveConversation();
                break;
            case 4:
                CompleteStage();
                break;
        }
    }

    private void LoveRobotEntrance() 
    {
        switch (phase) 
        {
            case 0:
                CreateRobotAfterTime(0.01f, loveRobotSpawnLocations[Random.Range(0, loveRobotSpawnLocations.Length)].position);
                CompletePhaseAfterSeconds(2);
                break;
            
            case 1:
                //Start patrol and Open head
                foreach (LoveRobot lR in loveRobots)
                {
                    lR.SetOpenBody(true);
                }
                CompletePhaseAfterSeconds(2);
                break;
            case 2:
                CompleteStage();
                break;
        }
    }

    private void CombatStage_1() 
    {
        switch(phase)
        {
            case 0:
                print("stuff");
                CompletePhaseAfterSeconds(10);
                break;
            case 1:
                print("stuff2");
                CompletePhaseAfterSeconds(5);
                break;
            case 2:
                print("stuff3");
                CompletePhaseAfterSeconds(5);
                break;

        }
    }


    private void PhaseUpdate()
    {
        switch (stage)
        {
            case 0:
                IntroStageUpdate();
                break;
            case 2:
                FireProjectilePatterns();
                break;

        }
    }

    private void IntroStageUpdate() 
    {
        if (phase == 1) LightPattern_CandlesOn();
    }


    private void CompleteStage()
    {
        stage++;
        phase = 0;
        stageCompleted = true;
    }

    /// <summary>
    /// 
    /// </summary>

    private void IntiateLoveConversation() 
    {
        if (npcConvoCount < loveNpcData.DialogueRootings.Count) 
        {
            isFiringPhase = false;
            DialogueManager.current.LoadDialogueTree(loveNpcData.DialogueObjectName, loveNpcData.DialogueRootings[npcConvoCount].Name);
            npcConvoCount++;
        }
    }

    //Lighting

    public void LightPattern_CandlesOn() 
    {
        if (allCandlesOn) return;

        LovePlatform leftMostPlatform = platforms[0];
        LovePlatform rightMostPlatform = platforms[platforms.Length - 1];

        if (currentShotTime >= 1.0f)
        {
            if (!leftMostPlatform.leftCandle.lightOn || !rightMostPlatform.rightCandle.lightOn)
            {
                leftMostPlatform.leftCandle.lightOn = true;
                rightMostPlatform.rightCandle.lightOn = true;
                lightDelayMultiplier *= 2f;
            }
            else if (!leftMostPlatform.rightCandle.lightOn || !rightMostPlatform.leftCandle.lightOn)
            {
                leftMostPlatform.rightCandle.lightOn = true;
                rightMostPlatform.leftCandle.lightOn = true;
                lightDelayMultiplier *= 2f;
            }
            else 
            {
                LovePlatform middlePlatform = platforms[platforms.Length / 2];

                if (!middlePlatform.leftCandle.lightOn)
                {
                    middlePlatform.leftCandle.lightOn = true;
                }
                else if (!middlePlatform.rightCandle.lightOn)
                {
                    middlePlatform.rightCandle.lightOn = true;
                }
                else 
                {
                    allCandlesOn = true;
                }

                lightDelayMultiplier *= 2f;
            }

            currentShotTime = 0f;
        }
        else
        {
            currentShotTime += Time.deltaTime * lightDelayMultiplier;
        }
    }

    //

    private LoveRobot CreateLoveRobot(Vector3 spawnLocation)
    {
        if (loveRobotSpawnLocations != null && loveRobotSpawnLocations.Any())
        {
            //Spawn this robot at a random spawn location
            LoveRobot loveRobot = Instantiate(loveRobotPrefab, spawnLocation, Quaternion.identity);

            //Add this robot to the list of robots
            if (!loveRobots.Contains(loveRobot))
                loveRobots.Add(loveRobot);


            return loveRobot;
        }

        return null;
    }

    //Phase changers

    public Task CompletePhaseAfterSeconds(float waitTime, float transitionTime = 1f)
    {
        IEnumerator completeAfterSecondsCoroutine()
        {
            yield return new WaitForSeconds(waitTime);

            phaseTransition = true;
            //ClearAllFiringCoroutines();
            PhaseTransitionTime(transitionTime);
        }

        return new Task(completeAfterSecondsCoroutine());
    }
    private Task MoveToPhaseAfterSeconds(int phase, float waitTime)
    {
        IEnumerator moveAfterSecondsCoroutine()
        {
            yield return new WaitForSeconds(waitTime);
            this.phase = phase;
            phaseCompleted = true;
        }

        return new Task(moveAfterSecondsCoroutine());
    }

    private Task PhaseTransitionTime(float waitTime)
    {
        IEnumerator phaseTransitionCoroutine()
        {
            yield return new WaitForSeconds(waitTime);
            if (!GameManager.current.GameIsOver)
            {
                phase++;
                phaseTransition = false;
                currentShotTime = 1f;
                phaseCompleted = true;
                Debug.Log("Phase " + phase + "!");
            }
        }

        return new Task(phaseTransitionCoroutine());
    }

    //Robot Coroutines
    private Task CreateRobotAfterTime(float waitTime, Vector3 spawnLocation)
    {
        IEnumerator createRobotAfterTimeCoroutine()
        {
            yield return new WaitForSeconds(waitTime);
            LoveRobot createdRobot = CreateLoveRobot(spawnLocation);

            if (loveRobotMovementPoints != null && loveRobotMovementPoints.Any())
            {
                //Find closest movement point if there is any
                Vector3 closestMovementPoint = loveRobotMovementPoints.OrderBy(x => Vector3.Distance(x.position, createdRobot.transform.position)).Select(x => x.position).FirstOrDefault();
                MoveRobotAfterTimeToLocation(0.1f, createdRobot, closestMovementPoint, true);
            }
        }

        return new Task(createRobotAfterTimeCoroutine());
    }

    private Task MoveRobotAfterTimeToLocation(float waitTime, LoveRobot loveRobot, Vector3 location, bool fastSpeed = false)
    {
        IEnumerator moveRobotCoroutine()
        {
            yield return new WaitForSeconds(waitTime);
            loveRobot.SetNewDestination(location, fastSpeed);
        }
        return new Task(moveRobotCoroutine());
    }

    private void FireAtTarget(Vector3 target)
    {
        loveRobots[Random.Range(0, loveRobots.Count)].LaunchProjectileAtTarget(target);
    }

}
