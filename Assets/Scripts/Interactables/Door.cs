using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Door : Interactable
{
    public bool isLit = true;
    public bool includeBackgroundInFade = true;
    public bool includePointLightSource = true;
    public SpriteRenderer backgroundSpriteRender;
    public SpriteRenderer primarySpriteRenderer;
    public Light2D light2D;
    public DoorName doorName;
    public DoorSceneRootingObject doorSceneRootingObj;

    private float targetAlpha = 1f;

    private float defaultLightIntensity;
    private float targetLightIntensity;
    private float maxSpeed = 300f;
    private float currentSpeed;
    private float targetSpeed;

    private DoorSceneRooting doorSceneRooting;

    private new void Awake()
    {
        base.Awake();
        if (light2D != null) defaultLightIntensity = light2D.intensity;
        targetLightIntensity = defaultLightIntensity;
        currentSpeed = maxSpeed;
        targetSpeed = currentSpeed;

        if (doorSceneRootingObj != null) doorSceneRooting = doorSceneRootingObj.doorSceneRootings.Where(x => x.doorName == doorName).First();
    }

    private new void Start()
    {
        base.Start();
    }

    private void Update()
    {
        targetAlpha = isLit ? 1f : 0f;
        targetLightIntensity = isLit ? defaultLightIntensity : 0.03f;
        targetSpeed = isLit ? maxSpeed : 0f;

        if (primarySpriteRenderer != null && primarySpriteRenderer.color.a != targetAlpha) 
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
        Debug.Log(gameObject.name + " interacted! This will take us to a level eventually!");

        if (doorSceneRooting != null && !string.IsNullOrEmpty(doorSceneRooting.sceneName))
        {
            GameManager.current.ChangeLevel(doorSceneRooting.sceneName);
        }
        else 
        {
            //This is just for interaction test purposes!
            isLit = !isLit;
        }
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