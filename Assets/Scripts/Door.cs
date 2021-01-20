using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Door : MonoBehaviour
{
    public bool isLit = true;
    public bool includeBackgroundInFade = true;
    public SpriteRenderer backgroundSpriteRender;

    private SpriteRenderer primarySpriteRenderer;

    private float targetAlpha = 1f;

    private void Awake()
    {
        primarySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        targetAlpha = isLit ? 1f : 0f;

        if (primarySpriteRenderer.color.a != targetAlpha) 
        {
            Color newColor = new Color(1f,1f,1f);
            newColor.a = Mathf.Lerp(primarySpriteRenderer.color.a, targetAlpha, 1f * Time.deltaTime);
            if (backgroundSpriteRender != null && includeBackgroundInFade) backgroundSpriteRender.color = newColor;
            primarySpriteRenderer.color = newColor;
        }


    }



}
