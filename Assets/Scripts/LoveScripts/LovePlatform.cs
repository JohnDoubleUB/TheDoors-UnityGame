using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LovePlatform : MonoBehaviour
{
    public Transform TargetLeft;
    public Transform TargetRight;

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
        
    }
}
