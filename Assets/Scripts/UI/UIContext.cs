using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContext : MonoBehaviour
{
    [HideInInspector]
    public UIContextType type;
    private void Start()
    {
        if (UIManager.current != null) UIManager.current.AssignObjectContext(this);
    }
}
