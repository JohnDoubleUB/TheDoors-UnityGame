using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleReturnPortal : Interactable
{
    public string sceneToReturnTo;

    public override void Interact()
    {
        GameManager.current.ChangeLevel(sceneToReturnTo);
    }
}
