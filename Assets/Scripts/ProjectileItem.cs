using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileItem : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.current != null)
        {
            Debug.Log("Thing"); //TODO: We want some collion to occur with the player so they can pick this up?

        }
        //Destroy(gameObject);
    }
}
