using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float timeDelay = 1f;
    private float currentTime = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.current.PlaySoundEvent("Jump", gameObject);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (sprite.sortingOrder != 4) 
        {
            if (currentTime < timeDelay)
            {
                currentTime += Time.deltaTime * 100;
            }
            else 
            {
                sprite.sortingOrder = 5;
            }
        }
    }
}
