using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : Interactable
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

    }

    new void Awake() 
    {
        base.Awake();

    }
    public override void Interact() 
    {
        Debug.Log("npc interact!");
    }
}
