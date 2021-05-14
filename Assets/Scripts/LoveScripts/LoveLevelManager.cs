using System.Collections;
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
    private bool patternTimerUpdated = false;
    private bool patternStageLooped = false;
    private Task patternChangerTask;

    public int stage = 0;
    public int phase = 0;
    public int playerPointPositionIndex;

    private float testIndexCounter = 0; //This shouldn't have "test on the start of it"
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
        //Get a middle platform point
        playerPointPositionIndex = platformPoints.Count / 2;

        //Get the player gameObject
        Player currentPlayer = GameManager.current.player;

        //Set the position of the player https://forum.unity.com/threads/drag-and-drop-streaming-asset-to-inspector-to-get-file-path.499055/
        currentPlayer.gameObject.transform.position = platformPoints[playerPointPositionIndex].position;
        currentPlayer.gameObject.transform.parent = platformPoints[playerPointPositionIndex];

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

    private void FireProjectilePatterns2() 
    {
        if (!patternTimerUpdated) 
        {
            patternChangerTask = ChangePatternAfterSeconds(testPatternManagerV2.CurrentPattern.PatternDuration); //Needs testing but this should happen once per pattern! 
            //We may also want to cancel these because when we change stage we will need to

            Debug.Log("Pattern " + testPatternManagerV2.CurrentPattern.Name);
        }

        if (testIndexCounter >= 1.0f)
        {
            Transform target = testPatternManagerV2.GetCurrentProjectileTargetTransform(patternStageLooped);
            FireAtTarget(target.position, patternStageLooped && testPatternManagerV2.CurrentProjectile.IsStageItem);
            //if (testPatternManagerV2.CurrentProjectile.IsStageItem) Debug.Log("is stage item!");

            testPatternManagerV2.NextProjectileTarget(patternStageLooped);
            testIndexCounter = 0f;
        }
        else
        {
            testIndexCounter += Time.deltaTime * testPatternManagerV2.CurrentDelay;
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
                    //CombatStage_1(); //We dont want to do this
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
                FireProjectilePatterns2();
                //FireProjectilePhase(); //This will handle all the firing of projectiles
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

    public Task ChangePatternAfterSeconds(float waitTime) 
    {
        IEnumerator changePatternAfterSecondsCoroutine() 
        {
            yield return new WaitForSeconds(waitTime);
            //Do we want transition time in this code?

            if (testPatternManagerV2 != null && !testPatternManagerV2.SetToNextPattern() && !patternStageLooped) 
            {
                patternStageLooped = true;
            }

            patternTimerUpdated = false;
        }

        patternTimerUpdated = true;

        return new Task(changePatternAfterSecondsCoroutine());
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

    private void FireAtTarget(Vector3 target, bool item = false)
    {
        if (item)
        {
            loveRobots[Random.Range(0, loveRobots.Count)].LaunchItemProjectileAtTarget(target);
        }
        else 
        {
            loveRobots[Random.Range(0, loveRobots.Count)].LaunchProjectileAtTarget(target);
        }
    }

}
