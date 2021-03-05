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

    //Hide in inspector?
    public List<Transform> platformPoints = new List<Transform>();

    //Each phases time
    public float[] phaseTimings = { 5, 2, 5, 5, 5, 15, 10, 6 };

    public float shotSpeed = 0.5f;
    private float currentShotTime = 0f;
    //public Transform[] playerMovementPoints

    public LoveRobot loveRobotPrefab;

    public List<LoveRobot> loveRobots = new List<LoveRobot>();

    private bool phaseCompleted = false;
    private bool phaseTransition = false;
    private bool alternator = false;
    public int phase = 0;
    public int playerPointPositionIndex;

    // Start is called before the first frame update

    private List<Task> activeFiringCoroutines = new List<Task>();


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

        //Set the position of the player
        currentPlayer.gameObject.transform.position = platformPoints[playerPointPositionIndex].position;
        currentPlayer.gameObject.transform.parent = platformPoints[playerPointPositionIndex];

        Debug.Log("Start!");
        StartPhase(0);
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

    // Update is called once per frame
    void Update()
    {
        if (!phaseTransition)
        {
            if (phaseCompleted)
            {
                StartPhase(phase);
                phaseCompleted = false;
            }

            UpdatePhase(phase);
        }
    }

    private void StartPhase(int phase)
    {
        //Set specific phase properties
        switch (phase)
        {
            case 0:
                CreateRobotAfterTime(0.1f, loveRobotSpawnLocations[Random.Range(0, loveRobotSpawnLocations.Length)].position);
                break;
            case 1:
                //Start patrol and Open head
                foreach (LoveRobot lR in loveRobots)
                {
                    lR.SetOpenBody(true);
                }
                break;
        }

        //Set timings to change the phase!
        if(phase < phaseTimings.Length) CompletePhaseAfterSeconds(phaseTimings[phase]);
    }

    private void UpdatePhase(int phase)
    {
        float phaseShotSpeedMultiplier = shotSpeed; //phase == 3 || phase == 5 ? shotSpeed * 1.5f : shotSpeed;

        switch (phase)
        {
            case 2:
                if (currentShotTime >= 1.0f)
                {
                    FireAtTarget(platformPoints[playerPointPositionIndex].position);
                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * phaseShotSpeedMultiplier;
                }
                break;

            case 3:
                if (currentShotTime >= 1.0f)
                {
                    FireAtTarget(platformPoints[playerPointPositionIndex].position);

                    //Hit the other point on that platform
                    int nextPosition = playerPointPositionIndex % 2 == 0 ? playerPointPositionIndex + 1 : playerPointPositionIndex - 1;
                    FireAtTarget(platformPoints[nextPosition].position, 0.1f);

                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * phaseShotSpeedMultiplier;
                }
                break;

            case 4:
                if (currentShotTime >= 1.0f)
                {
                    FireAtTarget(platformPoints[playerPointPositionIndex].position);

                    //Hit the other point on that platform
                    int nextPosition = playerPointPositionIndex % 2 == 0 ? playerPointPositionIndex + 1 : playerPointPositionIndex - 1;
                    FireAtTarget(platformPoints[nextPosition].position, 0.1f);

                    int position3;
                    int position4;

                    if (nextPosition == platformPoints.Count - 1 || playerPointPositionIndex == platformPoints.Count - 1 || nextPosition == 0 || playerPointPositionIndex == 0)
                    {
                        position3 = platformPoints.Count - 3;
                        position4 = position3 - 1;
                    }
                    else
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            position3 = platformPoints.Count - 1;
                            position4 = position3 - 1;
                        }
                        else
                        {
                            position3 = 0;
                            position4 = position3 + 1;
                        }
                    }

                    //TODO: Delays might need to have transform arguments rather than positional later
                    FireAtTarget(platformPoints[position3].position, 0.2f);
                    FireAtTarget(platformPoints[position4].position, 0.3f);

                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * phaseShotSpeedMultiplier;
                }
                break;

            case 5:

                if (currentShotTime >= 1.0f)
                {
                    float fireDelay = 0.3f;
                    Vector3[] platformPositions = platformPoints.Select(x => x.position).ToArray();
                    foreach (Vector3 target in alternator ? platformPositions : platformPositions.Reverse())
                    {
                        FireAtTarget(target, fireDelay);
                        fireDelay += 0.3f;
                    }

                    alternator = !alternator;
                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * phaseShotSpeedMultiplier;
                }
                break;
            case 6:
                if (currentShotTime >= 1.0f)
                {
                    float fireDelay = 0.3f;
                    Vector3[] platformPositions = platformPoints.Select(x => x.position).ToArray();

                    for (int i = 0; i < (platformPositions.Length / 2) + 1; i++)
                    {

                        FireAtTarget(platformPositions[i], fireDelay);
                        fireDelay += 0.3f;
                    }

                    fireDelay = 0.3f;

                    for (int i = platformPositions.Length - 1; i > (platformPositions.Length / 2); i--)
                    {
                        FireAtTarget(platformPositions[i], fireDelay);
                        fireDelay += 0.3f;
                    }

                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * (phaseShotSpeedMultiplier / 2f);
                }
                break;
            case 7:
                if (currentShotTime >= 1.0f)
                {
                    Vector3[] platformPositions = platformPoints.Select(x => x.position).ToArray();

                    FireAtTarget(platformPositions[playerPointPositionIndex]);
                    FireAtTarget(platformPositions[Random.Range(0, platformPositions.Length)], 0.1f);
                    FireAtTarget(platformPositions[Random.Range(0, platformPositions.Length)], 0.2f);

                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * phaseShotSpeedMultiplier;
                }
                break;
            case 8:

                break;

        }
    }

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

    private Task CompletePhaseAfterSeconds(float waitTime, float transitionTime = 1f)
    {
        IEnumerator completeAfterSecondsCoroutine()
        {
            yield return new WaitForSeconds(waitTime);

            phaseTransition = true;
            ClearAllFiringCoroutines();
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

    private void ClearAllFiringCoroutines()
    {
        foreach (Task firingTask in activeFiringCoroutines)
        {
            if (!firingTask.Running) continue;
            firingTask.Stop();
        }

        activeFiringCoroutines.Clear();
    }

    private void FireAtTarget(Vector3 target)
    {
        loveRobots[Random.Range(0, loveRobots.Count)].LaunchProjectileAtTarget(target);
    }

    private Task FireAtTarget(Vector3 target, float fireDelay)
    {
        IEnumerator fireAfterDelayCoroutine()
        {
            yield return new WaitForSeconds(fireDelay);
            FireAtTarget(target);
        }

        Task delayedFire = new Task(fireAfterDelayCoroutine());

        activeFiringCoroutines.Add(delayedFire);

        if (activeFiringCoroutines.Count > 10)
        {
            activeFiringCoroutines = activeFiringCoroutines.Where(x => x.Running).ToList();
        }

        return delayedFire;
    }
}
