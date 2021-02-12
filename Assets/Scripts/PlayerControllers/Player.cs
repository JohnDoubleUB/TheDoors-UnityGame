using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour //This could be an interface as all things are abstract?
{
    [HideInInspector]
    public bool Crouch;
    [HideInInspector]
    public bool JumpHold;

    public abstract void Jump();
    public abstract void Interact();
    public abstract void Move(Vector2 movement);
}
