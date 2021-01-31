using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class Door : Interactable
{
    public bool isLit = true;
    public bool includeBackgroundInFade = true;
    public bool includePointLightSource = true;
    public SpriteRenderer backgroundSpriteRender;
    public Light2D light2D;
    public DoorName doorName;


    private SpriteRenderer primarySpriteRenderer;

    private float targetAlpha = 1f;

    private float defaultLightIntensity;
    private float targetLightIntensity;
    private float maxSpeed = 300f;
    private float currentSpeed;
    private float targetSpeed;

    private new void Awake()
    {
        base.Awake();
        primarySpriteRenderer = GetComponent<SpriteRenderer>();
        if (light2D != null) defaultLightIntensity = light2D.intensity;
        targetLightIntensity = defaultLightIntensity;
        currentSpeed = maxSpeed;
        targetSpeed = currentSpeed;
    }

    private new void Start()
    {
        base.Start();
        if (GameManager.current != null) GameManager.current.AddDoor(this);
    }

    private void Update()
    {
        targetAlpha = isLit ? 1f : 0f;
        targetLightIntensity = isLit ? defaultLightIntensity : 0.03f;
        targetSpeed = isLit ? maxSpeed : 0f;

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

        //Make light go spin(?)
        if (currentSpeed != targetSpeed) 
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 1f * Time.deltaTime);

        if(currentSpeed != 0f)
            light2D.gameObject.transform.Rotate(new Vector3(0f, 0f, 1f) * currentSpeed * Time.deltaTime);
    }

    public override void Interact()
    {
        //Debug.Log(gameObject.name + " interacted!");

        //This is just for interaction test purposes!
        isLit = !isLit;
    }

}

public enum DoorName 
{
    Circus,
    Large,
    Sub,
    Portal,
    Arch,
    Curtain,
    Gate,
    Tent,
    Saloon,
    Trap,
    Garage
}