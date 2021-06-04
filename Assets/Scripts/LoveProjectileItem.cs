using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveProjectileItem : MonoBehaviour
{
    public float speedMultiplier = 1f;
    public float archHeight = 6f;
    public float rotationTime = 100f;

    private float smoothRot;

    private BoxCollider2D bc;
    private Rigidbody2D rb;

    private bool isAttatchedToPlayer;
    private bool fireAtTarget;
    private bool targetToRight;
    private float count;

    private Vector3 oldPosition;
    private Vector3 archPosition;
    private Transform newPositionTransform;

    public bool IsAttatchedToPlayer { get { return isAttatchedToPlayer; } }
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (fireAtTarget)
        {
            if (count < 1.0f)
            {
                count += 1.0f * (Time.deltaTime * speedMultiplier);
                Vector3 m1 = Vector3.Lerp(oldPosition, archPosition, count);
                Vector3 m2 = Vector3.Lerp(archPosition, newPositionTransform.position, count);
                transform.position = Vector3.Lerp(m1, m2, count);
                
                smoothRot = Time.deltaTime * rotationTime * 100f;
                transform.Rotate(new Vector3(0, 0, targetToRight ? -1 : 1), smoothRot);
            }
            else 
            {
                fireAtTarget = false;
                LoveLevelManager.current.HitLoveRobot();
                Destroy(gameObject);
            }
        }
    }

    public void Pickup() 
    {
        isAttatchedToPlayer = true;
        bc.enabled = false;
        rb.simulated = false;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void FireAtTarget(Transform position) 
    {
        Debug.Log("Fire at target!");
        newPositionTransform = position;
        oldPosition = transform.position;
        archPosition = oldPosition + (newPositionTransform.position - oldPosition) / 2 + Vector3.up * archHeight;
        count = 0f;
        isAttatchedToPlayer = false;
        targetToRight = position.position.x > transform.position.x;
        fireAtTarget = true;
    }



}
