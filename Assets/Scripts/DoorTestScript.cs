using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTestScript : MonoBehaviour
{
    private Door doorScript;

    private void Awake()
    {
        doorScript = GetComponent<Door>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        doorScript.isLit = false;//!doorScript.isLit;
    }
}
