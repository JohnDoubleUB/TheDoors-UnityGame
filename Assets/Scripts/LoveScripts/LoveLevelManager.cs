using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoveLevelManager : MonoBehaviour
{
    public Transform[] loveRobotSpawnLocations;
    public Transform[] loveRobotMovementPoints;

    public LoveRobot loveRobotPrefab;

    public List<LoveRobot> loveRobots = new List<LoveRobot>();

    private bool phaseCompleted = false;
    private int phase = 0;

    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
        //LoveRobot test = CreateLoveRobot();
        //IEnumerator robot1 = CreateRobotAfterTime(0.1f, loveRobotSpawnLocations[Random.Range(0, loveRobotSpawnLocations.Length)].position);
        //IEnumerator robot2 = CreateRobotAfterTime(0.3f, loveRobotSpawnLocations[loveRobotSpawnLocations.Length - 1].position);

        StartCoroutine(CreateRobotAfterTime(0.1f, loveRobotSpawnLocations[Random.Range(0, loveRobotSpawnLocations.Length)].position));
        //StartCoroutine(CreateRobotAfterTime(0.2f, loveRobotSpawnLocations[Random.Range(0, loveRobotSpawnLocations.Length)].position));
        StartCoroutine(StartNewPhaseAfter(5f));
        //StartCoroutine(robot2);

        
        Debug.Log("Start!");
    }

    // Update is called once per frame
    void Update()
    {
        if (phaseCompleted)
        {
            StartNextPhase();
            phaseCompleted = false;
        }
    }

    private void StartNextPhase()
    {
        phase++;
        switch (phase) 
        {
            case 1:
                //Start patrol and Open head
                foreach (LoveRobot lR in loveRobots) 
                {
                    lR.SetOpenBody(true);
                }
                Debug.Log("Started Phase1!");
                break;
        }
    }

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

    private IEnumerator StartNewPhaseAfter(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        phaseCompleted = true;
    }

    private IEnumerator MoveRobotAfterTimeToLocation(float waitTime, LoveRobot loveRobot, Vector3 location, bool fastSpeed = false) 
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("MoveRobotAfterTimeToLocation");
        loveRobot.SetNewDestination(location, fastSpeed);
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
}
