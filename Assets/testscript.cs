using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    private float smoothRot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        smoothRot = Time.deltaTime * 5f * 100f;
        transform.Rotate(new Vector3(0, 0, 1), smoothRot);
    }
}
