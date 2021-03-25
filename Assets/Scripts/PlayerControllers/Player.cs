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

    public int maxHealth = 3;
    
    private int currentHealth = -1;

    public int CurrentHealth 
    {
        get { return currentHealth < 0 ? maxHealth : currentHealth; } 
    }


    protected List<Interactable> interactables = new List<Interactable>();
    protected Interactable closestInteractable;

    public virtual void Jump() { }
    public virtual void Interact() { }
    public virtual void Move(Vector2 movement) { }
    public virtual void OnFootFall() { }
    public virtual void MoveOnceInDirection(InputMapping input) { }
    public virtual void TakeDamage(int damageAmount = 1) 
    {
        //Ensure that the number is actually positive (we don't want to add health with take damage)
        int damageAmountAbs = Mathf.Abs(damageAmount);
        
        if (currentHealth - damageAmountAbs < 0) 
        { 
            currentHealth = 0; 
        }
        else 
        {
            currentHealth -= damageAmountAbs;
        }

        GameManager.current.UpdateHealth(currentHealth);
    }
    protected void Update()
    {
        if (interactables.Any()) SelectClosestInteractable();
    }

    protected void Awake()
    {
        currentHealth = maxHealth;
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