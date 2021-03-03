using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveRobot : MonoBehaviour
{
    public float distance = 0.3f;
    public float fastMovementMultiplier = 20f;
    public float floatSpeed = 1f;
    public float projectileAirTime = 1.2f;
    public float projectileRecoilAmount = -0.5f;

    public Rigidbody2D projectilePrefab;
    //public Transform testTarget; //TODO: Remove this
    public Transform projectileSpawnPoint;

    //These are just the parts that we need to have access to
    public Transform propeller;
    public Animator animator;

    private Vector3 currentLocation;
    private Vector3 targetLocation;
    private float transition = 1f;
    private float recoil = 0f;

    private bool fastMovement = false;
    private bool isAtTarget = true;
    private float randomFloatSeed = 0f;
    private int randomIntSeed = 0;

    public bool BodyIsOpen
    {
        get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("open"); }
    }

    public bool BodyIsClosed
    {
        get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("closed"); }
    }

    public bool BodyIsInTransition 
    {
        get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("transtition"); } 
    }

    public bool IsAtTarget 
    {
        get { return isAtTarget; } 
    }

    public Vector3 TargetLocation 
    {
        get { return targetLocation; } 
    }

    private void Awake()
    {
        currentLocation = transform.position;
        targetLocation = currentLocation;

        randomFloatSeed = Random.Range(1f, 3f);
        randomIntSeed = Random.Range(0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        //Generate our floaty effect!
        Vector2 floatValue = GenerateFloat((Time.time + randomIntSeed) * distance, floatSpeed);

        //Move towards target if target isn't the current location
        float distanceToTarget = Vector3.Distance(currentLocation, targetLocation);
        isAtTarget = distanceToTarget > 0.1f;
        
        if (isAtTarget)
        {
            currentLocation = Vector3.Lerp(currentLocation, targetLocation, transition);
            transition += (Time.deltaTime * (fastMovement ? 0.005f * fastMovementMultiplier : 0.005f) / distanceToTarget);
        }
        else if (transition != 0f)
        {
            transition = 0f;
            fastMovement = false;
        }

        //Set location and rotation of the body
        transform.position = new Vector3(currentLocation.x + floatValue.x, currentLocation.y + floatValue.y + recoil, 0);//Vector3.up * Mathf.Sin(Time.time * 0.01f + 100 * Time.time);
        transform.rotation = Quaternion.Euler(0, 0, 4 * floatValue.x);

        //Set rotation of the propeller
        if (propeller != null) propeller.rotation = Quaternion.Euler(0, 0, 10 * floatValue.x);

        //Set recoil up
        if (recoil < 0f)
        {
            recoil += Time.deltaTime * 5;
        }
        else
        {
            recoil = 0f;
        }
    }

    public void LaunchProjectileAtTarget(Vector3 target)
    {
        Rigidbody2D obj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        obj.gravityScale = Random.Range(2f, 4f);
        Vector3 Vo = CalculateVelocity(target, projectileSpawnPoint.position, Random.Range(projectileAirTime, projectileAirTime * 1.2f), obj.gravityScale);
        obj.velocity = Vo;
        obj.angularVelocity += Vo.x > 0 ? -500 : 500;
        //Add recoil when firing projectiles
        recoil = projectileRecoilAmount;
    }

    public void SetNewDestination(Vector3 destination, bool fastMovement = false) 
    {
        this.fastMovement = fastMovement;
        targetLocation = destination;
    }

    public void SetOpenBody(bool open) 
    {
        animator.SetBool("bodyOpen", open);
    }

    private Vector2 GenerateFloat(float time, float floatSpeed) 
    {
        //Add floatyness
        Vector2 generatedFloat = Floaty(time);

        //Add wavyness
        generatedFloat += Wavy(time, floatSpeed);

        return generatedFloat;
    }

    private Vector2 Floaty(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f * randomFloatSeed), Mathf.Cos(time * 2.5f));
    }

    private Vector2 Wavy(float time, float speed = 10)
    {
        return Vector2.up * Mathf.Sin(time * 0.1f + speed * time); //* 0.04f;
    }

    private Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time, float gravityScale = 1f)
    {
        //define the distance x and y first

        Vector3 distance = target - origin;
        Vector3 distance_x_z = distance;
        distance_x_z.Normalize();
        distance_x_z.y = 0;

        //creating a float that represents our distance
        float sy = distance.y;
        float sxz = distance.magnitude;

        //calculating initial x velocity
        //Vx = x / t

        float Vxz = sxz / time;
        ////calculating initial y velocity

        //Vy0 = y/t + 1/2 * g * t

        float Vy = sy / time + 0.5f * Mathf.Abs(Physics2D.gravity.y * gravityScale) * time;

        Vector3 result = distance_x_z * Vxz;

        result.y = Vy;
        return result;

    }

}
