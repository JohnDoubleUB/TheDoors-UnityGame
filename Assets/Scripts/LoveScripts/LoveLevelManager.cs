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


    public float shotSpeed = 0.5f;
    private float currentShotTime = 0f;
    //public Transform[] playerMovementPoints

    public LoveRobot loveRobotPrefab;

    public List<LoveRobot> loveRobots = new List<LoveRobot>();

    private bool phaseCompleted = false;
    public int phase = 0;
    public int playerPointPositionIndex;

    // Start is called before the first frame update

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
        if (phaseCompleted)
        {
            StartPhase(phase);
            phaseCompleted = false;
        }

        UpdatePhase(phase);
    }

    private void StartPhase(int phase)
    {
        switch (phase) 
        {
            case 0:
                StartCoroutine(CreateRobotAfterTime(0.1f, loveRobotSpawnLocations[Random.Range(0, loveRobotSpawnLocations.Length)].position));
                StartCoroutine(CompletePhaseAfterSeconds(5f));
                break;
            case 1:
                //Start patrol and Open head
                foreach (LoveRobot lR in loveRobots) 
                {
                    lR.SetOpenBody(true);
                }
                StartCoroutine(CompletePhaseAfterSeconds(2f));
                //StartCoroutine(MoveToPhaseAfterSeconds(7, 2f));
                break;
            case 2:
                Debug.Log("Phase2!");
                StartCoroutine(CompletePhaseAfterSeconds(5f));
                break;
            case 3:
                Debug.Log("Phase3!");
                StartCoroutine(CompletePhaseAfterSeconds(5f));
                break;
            case 4:
                Debug.Log("Entering phase 4!");
                StartCoroutine(CompletePhaseAfterSeconds(5f));
                break;
            case 5:
                Debug.Log("Entering phase 5!");
                StartCoroutine(CompletePhaseAfterSeconds(10f));
                break;
            case 6:
                Debug.Log("Entering phase 6");
                StartCoroutine(CompletePhaseAfterSeconds(10f));
                break;
        }
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
                    StartCoroutine(FireAtTargetAfterDelay(platformPoints[nextPosition].position, 0.1f));

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
                    StartCoroutine(FireAtTargetAfterDelay(platformPoints[nextPosition].position, 0.1f));

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
                    StartCoroutine(FireAtTargetAfterDelay(platformPoints[position3].position, 0.2f));
                    StartCoroutine(FireAtTargetAfterDelay(platformPoints[position4].position, 0.3f));

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
                    foreach (Vector3 target in platformPositions) 
                    {
                        StartCoroutine(FireAtTargetAfterDelay(target, fireDelay));
                        fireDelay += 0.3f;
                    }

                    fireDelay += 1;

                    foreach (Vector3 target in platformPositions.Reverse()) 
                    {
                        StartCoroutine(FireAtTargetAfterDelay(target, fireDelay));
                        fireDelay += 0.3f;
                    }

                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * (phaseShotSpeedMultiplier / 3f);
                }
                break;
            case 6:
                if (currentShotTime >= 1.0f)
                {
                    float fireDelay = 0.3f;
                    Vector3[] platformPositions = platformPoints.Select(x => x.position).ToArray();
                    
                    for (int i = 0; i < (platformPositions.Length / 2) + 1; i++) 
                    {
                        StartCoroutine(FireAtTargetAfterDelay(platformPositions[i], fireDelay));
                        fireDelay += 0.3f;
                    }

                    fireDelay = 0.3f;

                    for (int i = platformPositions.Length - 1; i > (platformPositions.Length / 2); i--)
                    {
                        StartCoroutine(FireAtTargetAfterDelay(platformPositions[i], fireDelay));
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
                    float fireDelay = 0.3f;
                    Vector3[] platformPositions = platformPoints.Select(x => x.position).ToArray();

                    FireAtTarget(platformPositions[playerPointPositionIndex]);
                    StartCoroutine(FireAtTargetAfterDelay(platformPositions[Random.Range(0, platformPositions.Length)], 0.1f));
                    StartCoroutine(FireAtTargetAfterDelay(platformPositions[Random.Range(0, platformPositions.Length)], 0.2f));

                    currentShotTime = 0f;
                }
                else
                {
                    currentShotTime += Time.deltaTime * (phaseShotSpeedMultiplier);
                }
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


    //Start new phase

    private IEnumerator CompletePhaseAfterSeconds(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        phase++;
        phaseCompleted = true;
        currentShotTime = 0f;
        //currentShotTime = 1f;
    }

    private IEnumerator MoveToPhaseAfterSeconds(int phase, float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        this.phase = phase;
        phaseCompleted = true;
    }

    private IEnumerator CompletePhaseAfterMinutes(float waitTime)
    {
        yield return new WaitForSeconds(60f * waitTime);
        phase++;
        phaseCompleted = true;
    }

    //Robot Coroutines
    private IEnumerator CreateRobotAfterTime(float waitTime, Vector3 spawnLocation)
    {
        yield return new WaitForSeconds(waitTime);
        LoveRobot test = CreateLoveRobot(spawnLocation);
        Debug.Log("CreateRobotAfterTime");

        if (loveRobotMovementPoints != null && loveRobotMovementPoints.Any())
        {
            //Find closest movement point if there is any
            Vector3 closestMovementPoint = loveRobotMovementPoints.OrderBy(x => Vector3.Distance(x.position, test.transform.position)).Select(x => x.position).FirstOrDefault();

            IEnumerator subCoroutine = MoveRobotAfterTimeToLocation(0.1f, test, closestMovementPoint, true);
            StartCoroutine(subCoroutine);
        }
    }

    private IEnumerator MoveRobotAfterTimeToLocation(float waitTime, LoveRobot loveRobot, Vector3 location, bool fastSpeed = false) 
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("MoveRobotAfterTimeToLocation");
        loveRobot.SetNewDestination(location, fastSpeed);
    }

    private IEnumerator FireAtTargetAfterDelay(Vector3 target, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        FireAtTarget(target);
    }

    private void FireAtTarget(Vector3 target)
    {
        loveRobots[Random.Range(0, loveRobots.Count)].LaunchProjectileAtTarget(target);
    }
}
