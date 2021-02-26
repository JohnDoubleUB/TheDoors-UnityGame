using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IEnumerator test = TestCoroutine1(2.5f);
        StartCoroutine(test);
    }

    private IEnumerator TestCoroutine1(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("test");
        LeanTween.moveX(gameObject, transform.position.x + 10f, 1f).setEase( LeanTweenType.easeOutBack );
        //LeanTween.moveX(,)
    }
}
