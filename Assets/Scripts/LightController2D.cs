using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightController2D : MonoBehaviour
{
    public Light2D lightComponent;
    public bool lightOn = true;
    public float lightTransitionSpeed = 1f;
    public float lightTransitionAccuracy = 0f;
    public bool lightFlicker = false;
    public float flickerTime = 2f; //If this is set to negative then the light will always flicker

    private bool lightInTransition = false;
    private float defaultIntensity;
    private float targetLightIntensity;
    public float currentFlickerTime = 0f;
    private bool lastFrameFlicker;

    private void Awake()
    {
        lastFrameFlicker = lightFlicker;

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
            lightInTransition = true;
        }
        else if(!lightOn && targetLightIntensity != 0f)
        {
            targetLightIntensity = 0f;
            lightInTransition = true;
        }

        //Reset the flicker time whenever the flicker has been set on or off again
        if (lastFrameFlicker != lightFlicker) 
        {
            lastFrameFlicker = lightFlicker;
            currentFlickerTime = 0f;
        }


        bool lightIsCorrect = (lightOn && lightComponent.intensity < targetLightIntensity - lightTransitionAccuracy) || (!lightOn && lightComponent.intensity > targetLightIntensity);

        if (lightComponent != null && lightIsCorrect/*lightComponent.intensity != targetLightIntensity*/ && lightInTransition)
        {
            lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, targetLightIntensity, lightTransitionSpeed * Time.deltaTime);
        }
        else if(lightInTransition)
        {
            lightComponent.intensity = targetLightIntensity;
            lightInTransition = false;
        }

        //Add light flicker if thats what we want
        if (lightFlicker && lightOn && !lightInTransition && currentFlickerTime < flickerTime)
        {
            if (Random.value > 0.97) lightComponent.enabled = !lightComponent.enabled;
            if (flickerTime > 0f) currentFlickerTime += Time.deltaTime;
        }
        else if(currentFlickerTime >= flickerTime)
        {
            lightFlicker = false;
            lightComponent.enabled = true; //Ensure that the light always finishes as on
        }
    }


}
