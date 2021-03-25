using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContextObject : UIContext
{
    public UIContextType Type;

    private void Awake()
    {
        type = Type;
    }
}
