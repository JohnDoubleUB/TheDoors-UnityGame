using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerKillArea : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Player") && GameManager.current != null && GameManager.current.Player != null) 
            GameManager.current.Player.TakeDamage(999); 
    }
}
