using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIHealthContext : UIContext
{
    private Text text;
    private void Awake()
    {
        type = UIContextType.HealthDisplay;
        text = GetComponent<Text>();
    }

    public void SetHealth(int healthValue) 
    {
        if (text != null) text.text = "Health: " + healthValue;
    }
}
