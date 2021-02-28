using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LovePlatform : MonoBehaviour
{
    public Transform TargetLeft;
    public Transform TargetRight;
    public float recoilStrength = -1f;
    public float rotationRecoilStrength = 2f;

    private Vector3 defaultPosition;
    private Vector3 defaultRotationEuler;
    private float positionTransition = 0f;
    private float rotationTransition = 0f;

    private void Awake()
    {
        defaultPosition = transform.position;
        defaultRotationEuler = transform.rotation.eulerAngles;
    }

    public Transform[] LeftAndRight 
    {
        get 
        {
            return new Transform[] { TargetLeft, TargetRight }; 
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (transform.position != defaultPosition) 
        {
            transform.position = Vector3.Lerp(transform.position, defaultPosition, positionTransition);
            positionTransition += Time.deltaTime;
        }

        if (transform.rotation.eulerAngles != defaultRotationEuler)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(defaultRotationEuler), rotationTransition);
            rotationTransition += Time.deltaTime;
        }

    }

    public void TakeHit(Vector3 hitLocation, float hitStrengthMultiplier = 1f) 
    {
        positionTransition = 0f;
        rotationTransition = 0f;
        
        transform.position = new Vector3(
            defaultPosition.x, 
            defaultPosition.y + (recoilStrength * hitStrengthMultiplier), 
            defaultPosition.z
            );
        
        transform.rotation = Quaternion.Euler(
            defaultRotationEuler.x, 
            defaultRotationEuler.y, 
            defaultRotationEuler.z + (hitLocation.x > transform.position.x ? -rotationRecoilStrength * hitStrengthMultiplier : rotationRecoilStrength * hitStrengthMultiplier)
            );
    }
}
