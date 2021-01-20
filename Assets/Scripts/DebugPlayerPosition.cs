using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPlayerPosition : MonoBehaviour
{
    public Transform playerTransform;
    private Text t;

    private void Awake()
    {
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (t != null && playerTransform != null) 
        {
            string playerLocation = "(" + playerTransform.position.x + ", " + playerTransform.position.y + ")";
            t.text = playerLocation;
        }
    }
}
