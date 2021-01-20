﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class Door : MonoBehaviour
{
    public bool isLit = true;
    public bool includeBackgroundInFade = true;
    public bool includePointLightSource = true;
    public SpriteRenderer backgroundSpriteRender;
    public Light2D light2D;


    private SpriteRenderer primarySpriteRenderer;

    private float targetAlpha = 1f;

    private float defaultLightIntensity;
    private float targetLightIntensity;

    private void Awake()
    {
        primarySpriteRenderer = GetComponent<SpriteRenderer>();
        if (light2D != null) defaultLightIntensity = light2D.intensity;
        targetLightIntensity = defaultLightIntensity;
    }

    private void Update()
    {
        targetAlpha = isLit ? 1f : 0f;
        targetLightIntensity = isLit ? defaultLightIntensity : 0.03f;

        if (primarySpriteRenderer.color.a != targetAlpha) 
        {
            Color newColor = new Color(1f,1f,1f);
            float lerpFloat = Mathf.Lerp(primarySpriteRenderer.color.a, targetAlpha, 1f * Time.deltaTime);
            newColor.a = lerpFloat;
            //Seeing as all these values are between 0 and 1 I'm doing all at once
            if (backgroundSpriteRender != null && includeBackgroundInFade) backgroundSpriteRender.color = newColor;
            primarySpriteRenderer.color = newColor;
        }

        if (light2D != null && light2D.intensity != targetLightIntensity) 
            light2D.intensity = Mathf.Lerp(light2D.intensity, targetLightIntensity, 1f * Time.deltaTime);
        

    }



}
