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
    private Vector3 newPosition;
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
        oldPosition = transform.position;
        newPosition = LoveLevelManager.current.MovePlayerToNewPositionPoint(pointChange).position;
        archPosition = oldPosition + (newPosition - oldPosition) / 2 + Vector3.up * jumpArchHeight;
        count = 0.0f;
        moving = true;
        movingSound = true;
    }

    protected new void Update()
    {
        base.Update();

        if (moving) 
        {
            if (count < 1.0f)
            {
                count += 1.0f * (Time.deltaTime * speedMultiplier);

                Vector3 m1 = Vector3.Lerp(oldPosition, archPosition, count);
                Vector3 m2 = Vector3.Lerp(archPosition, newPosition, count);
                transform.position = Vector3.Lerp(m1, m2, count);
            }
            else
            {
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
