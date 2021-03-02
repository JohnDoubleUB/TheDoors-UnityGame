using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LovePlayer : Player
{
    public float speedMultiplier = 2f;
    public float jumpArchHeight = 2.5f;

    public SpriteRenderer playerSprite;

    private float count = 0.0f;
    private bool moving;
    private bool movingSound;
    private Vector3 oldPosition;
    private Vector3 archPosition;
    private Transform newPositionTransform;
    private float lightUpTimer = 1f;
    public override void MoveOnceInDirection(InputMapping input)
    {
        switch (input) 
        {
            case InputMapping.Left:
                MoveToNewPoint(-1);
                if (playerSprite != null) playerSprite.flipX = true;

                break;
            case InputMapping.Right:
                MoveToNewPoint(1);
                if (playerSprite != null) playerSprite.flipX = false;
                break;
        }
    }

    private void MoveToNewPoint(int pointChange) 
    {
        transform.rotation = Quaternion.identity;
        transform.parent = null;
        
        oldPosition = transform.position;
        newPositionTransform = LoveLevelManager.current.MovePlayerToNewPositionPoint(pointChange);
        archPosition = oldPosition + (newPositionTransform.position - oldPosition) / 2 + Vector3.up * jumpArchHeight;
        count = 0.0f;
        moving = true;
        movingSound = true;
    }

    public override void TakeDamage()
    {
        playerSprite.color = Color.red;
        lightUpTimer = 0f;
        PlayerHealth--;
    }

    protected new void Update()
    {
        base.Update();

        if (lightUpTimer < 1f)
        {
            lightUpTimer += Time.deltaTime * 10f;
        }
        else if (playerSprite.color != Color.white)
        {
            playerSprite.color = Color.white;
        }


        if (moving) 
        {
            if (count < 1.0f)
            {
                
                count += 1.0f * (Time.deltaTime * speedMultiplier);

                Vector3 m1 = Vector3.Lerp(oldPosition, archPosition, count);
                Vector3 m2 = Vector3.Lerp(archPosition, newPositionTransform.position, count);
                transform.position = Vector3.Lerp(m1, m2, count);
            }
            else
            {
                if (newPositionTransform != null && transform.parent != newPositionTransform) transform.parent = newPositionTransform;
                moving = false;
            }

            if (count > 0.85f && movingSound) 
            {
                AudioManager.current.PlaySoundEvent("Walk_Cycle", gameObject);
                movingSound = false;
            }
        }
    }
}
