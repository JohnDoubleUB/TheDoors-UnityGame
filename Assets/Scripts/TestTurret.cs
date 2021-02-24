using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTurret : MonoBehaviour
{
    public Rigidbody2D bulletPrefab;
    public Transform testTarget;
    public Transform shootPoint;
    public float time = 1.2f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LaunchProjectile();
    }

    void LaunchProjectile()
    {
        

        if (Input.GetMouseButtonDown(0))
        {   
            Rigidbody2D obj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            obj.gravityScale = Random.Range(2f, 4f);
            Vector3 Vo = CalculateVelocity(testTarget.position, shootPoint.position, Random.Range(time, time * 1.2f), obj.gravityScale);
            obj.velocity = Vo;
        }
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

    private Vector2 CalculateVelocity2(Vector2 target, Vector2 origin, float time)
    {
        //define the distance x and y first

        Vector2 distance = target - origin;
        Vector2 distanceNormalized = distance;
        distanceNormalized.Normalize();
        distanceNormalized.y = 0;

        //creating a float that represents our distance
        float sy = distance.y;
        float sx = distance.magnitude;

        //calculating initial x velocity
        //Vx = x / t

        float Vxz = sx / time;
        ////calculating initial y velocity

        //Vy0 = y/t + 1/2 * g * t

        float Vy = sy / time + 0.5f * Mathf.Abs(Physics2D.gravity.y) * time;

        Vector2 result = distanceNormalized * Vxz;

        result.y = Vy;

        Debug.Log(result);
        return result;
    }
}
