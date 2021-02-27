using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTestScript : MonoBehaviour
{

    private ParticleSystem particleSystem;


    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("yo");
            if (particleSystem != null) particleSystem.Play();
        }
    }
}
