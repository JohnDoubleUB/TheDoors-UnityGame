using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float timeDelay = 1f;
    public float damageRadius = 1.5f;
    private float currentTime = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.current.PlaySoundEvent("Jump", gameObject);
        if (LoveLevelManager.current != null) LoveLevelManager.current.SpawnParticleEffectAtPosition(transform.position);
        if (GameManager.current != null) 
        {
            if (Vector3.Distance(GameManager.current.player.transform.position, transform.position) <= 3f) 
            {
                Debug.Log("Player has been hit!");
            }
            
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        if (sprite.sortingOrder != 6) 
        {
            if (currentTime < timeDelay)
            {
                currentTime += Time.deltaTime * 100;
            }
            else 
            {
                sprite.sortingOrder = 6;
            }
        }
    }
}
