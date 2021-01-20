using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContextObject : MonoBehaviour
{
    public UIContextType type;

    private void Start()
    {
        if (UIManager.current != null) UIManager.current.AssignObjectContext(this);
    }
}
