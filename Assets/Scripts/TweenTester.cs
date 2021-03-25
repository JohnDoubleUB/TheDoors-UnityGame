using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenTester : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    private float animationTest;
    private bool start;
    private float count = 0.0f;
    private float speedMultiplier = 5f;

    private Vector3 point0, point1, point2;
    
    // Start is called before the first frame update
    void Start()
    {
        point0 = startPoint.position;
        point2 = endPoint.position;
        point1 = point0 + (point2 - point0) / 2 + Vector3.up * 5.0f; // Play with 5.0 to change the curve


        IEnumerator test = TestCoroutine1(2.5f);
        StartCoroutine(test);
        //point[1] = point[0] + (point[2] - point[0]) / 2 + Vector3.up * 5.0f; // Play with 5.0 to change the curve
    }

    private IEnumerator TestCoroutine1(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("test");
        start = true;
        //LeanTween.moveX(gameObject, transform.position.x + 10f, 1f).setEase( LeanTweenType.easeOutBack );
        //LeanTween.moveX(,)
    }

    private void Update()
    {
        if (start) 
        {
            if (count < 1.0f)
            {
                count += 1.0f * (Time.deltaTime * speedMultiplier);

                Vector3 m1 = Vector3.Lerp(point0, point1, count);
                Vector3 m2 = Vector3.Lerp(point1, point2, count);
                transform.position = Vector3.Lerp(m1, m2, count);
            }
        }
    }



}
