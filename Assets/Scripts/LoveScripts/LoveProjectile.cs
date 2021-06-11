using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveProjectile : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float timeDelay = 1f;
    public float damageRadius = 1f;
    private float currentTime = 0f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.current.PlaySoundEvent("Jump", gameObject);
        if (LoveLevelManager.current != null) 
        {
            LoveLevelManager.current.SpawnParticleEffectAtPosition(transform.position);
            
            if (LoveLevelManager.current.PickupItem != null && Vector3.Distance(LoveLevelManager.current.PickupItem.transform.position, transform.position) <= damageRadius) 
                Destroy(LoveLevelManager.current.PickupItem.gameObject); 
        }

        if (GameManager.current != null) 
        {
            if (Vector3.Distance(GameManager.current.player.transform.position, transform.position) <= damageRadius) 
            {
                GameManager.current.player.TakeDamage();
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
