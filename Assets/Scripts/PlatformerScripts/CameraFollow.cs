﻿using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        //Get the player transform from the GameManager!
        if (GameManager.current != null) target = GameManager.current.Player.transform;
    }

    private void LateUpdate()
    {
        //Where we want the camera
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        
        //The Linearly interpolated movement from current to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }


}