using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugUIText : MonoBehaviour
{
    public static DebugUIText current;
    
    
    private Text text;
    private void Awake()
    {
        if (current == null) current = this;
        text = GetComponent<Text>();
        //SetText("");
    }

    public void SetText(string text) 
    {
        this.text.text = text;
    }
}
