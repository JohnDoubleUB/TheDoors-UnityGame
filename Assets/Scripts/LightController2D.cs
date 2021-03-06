using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightController2D : MonoBehaviour
{
    public Light2D lightComponent;
    public bool lightOn = true;
    public float lightTransitionSpeed = 1f;

    private float defaultIntensity;
    private float targetLightIntensity;

    private void Awake()
    {
        if (lightComponent != null) 
        {
            defaultIntensity = lightComponent.intensity;
            targetLightIntensity = lightOn ? defaultIntensity : 0f;
            lightComponent.intensity = targetLightIntensity;
        }
    }

    private void Update()
    {
        if (lightOn && targetLightIntensity != defaultIntensity)
        {
            targetLightIntensity = defaultIntensity;
        }
        else if(!lightOn && targetLightIntensity != 0f)
        {
            targetLightIntensity = 0f;
        }

        if (lightComponent != null && lightComponent.intensity != targetLightIntensity)
            lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, targetLightIntensity, lightTransitionSpeed * Time.deltaTime);
    }


}
