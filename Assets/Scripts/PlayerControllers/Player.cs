using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [HideInInspector]
    public bool Crouch;
    [HideInInspector]
    public bool JumpHold;

    protected List<Interactable> interactables = new List<Interactable>();
    protected Interactable closestInteractable;

    public abstract void Jump();
    public abstract void Interact();
    public abstract void Move(Vector2 movement);

    protected void Update()
    {
        if (interactables.Any()) SelectClosestInteractable();
    }

    private void SelectClosestInteractable()
    {
        Interactable closestInteractable = null;
        float shortestDistance = float.MaxValue;
        float tempDistance;

        foreach (Interactable interObj in interactables)
        {
            interObj.Selected = false;

            tempDistance = Vector2.Distance(
                new Vector2(interObj.transform.position.x, interObj.transform.position.y),
                new Vector2(transform.position.x, transform.position.y)
                );

            if (tempDistance < shortestDistance)
            {
                shortestDistance = tempDistance;
                closestInteractable = interObj;
            }
        }

        closestInteractable.Selected = true;
        this.closestInteractable = closestInteractable;
    }

    public void AddInteractable(Interactable interactable)
    {
        if (!interactables.Contains(interactable)) interactables.Add(interactable);
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (interactables.Contains(interactable)) interactables.Remove(interactable);
        if (!interactables.Any()) closestInteractable = null;
    }
}
