using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float waitTime = 0.5f;
    private float currentWaitTime;


    private void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        //When user presses down platform collision allows them to fall back through
        if (Input.GetAxis("Vertical") < 0f)
        {
            if (currentWaitTime <= 0)
            {
                effector.rotationalOffset = 180f;
                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
            }
        }
        else if (effector.rotationalOffset != 0f) //If no longer holding down and rotationalOffset is still set from before set it back
        {
            currentWaitTime = waitTime;
            effector.rotationalOffset = 0f;
        }
    }
}
