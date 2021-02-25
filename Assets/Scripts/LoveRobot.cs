using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveRobot : MonoBehaviour
{
    public float distance = 0.2f;
    public float floatSpeed = 1f;
    public Vector3 location;
    public Vector3 target;

    public Rigidbody2D bulletPrefab;
    public Transform testTarget;
    public Transform shootPoint;
    public float time = 1.2f;

    public Transform propeller;
    public Animator animator;

    public Transform[] destinations;

    private Transform currentDestination;

    private float transition = 0f;
    private float recoilTransistion = 0f;

    private float recoil = 0f;
    //public Vector2
    // Start is called before the first frame update

    private void Awake()
    {
        location = transform.position;
        target = location;
    }

    void Start()
    {
        //target = destination.position;


        animator.SetBool("bodyOpen", true);
      
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 insideCircle = Random.insideUnitCircle * distance;
        Vector2 floatyFloat = Floaty(Time.time * distance);
        floatyFloat += Wavy(Time.time * distance, floatSpeed);

        if (LaunchProjectile()) 
        {
            //Debug.Log("thing");
            recoil = -0.5f;
            //floatyFloat += new Vector2(0, -20);
        }


        transform.position = new Vector3(location.x + floatyFloat.x, location.y + floatyFloat.y + recoil, 0);//Vector3.up * Mathf.Sin(Time.time * 0.01f + 100 * Time.time);
        transform.rotation = Quaternion.Euler(0, 0, 4 * floatyFloat.x);

        if(propeller != null) propeller.rotation = Quaternion.Euler(0, 0, 10 * floatyFloat.x);

        if (Vector3.Distance(location, target) > 0.1f)
        {
            location = Vector3.Lerp(location, target, transition);
            transition += (Time.deltaTime * 0.001f);
        }
        else
        {
            transition = 0f;
        }

        if (recoil < 0f)
        {
            recoil += Time.deltaTime * 5;
        }
        else 
        {
            recoil = 0f;
        }

        //if (animator.GetCurrentAnimatorStateInfo(0).IsTag("open"))
        //{
        //    Debug.Log("Open");
        //}

    }

    private Vector2 Floaty(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f));
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

    private bool LaunchProjectile()
    {


        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody2D obj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            obj.gravityScale = Random.Range(2f, 4f);
            Vector3 Vo = CalculateVelocity(testTarget.position, shootPoint.position, Random.Range(time, time * 1.2f), obj.gravityScale);
            obj.velocity = Vo;
            obj.angularVelocity += 500;
            return true;
        }

        return false;
    }

}
