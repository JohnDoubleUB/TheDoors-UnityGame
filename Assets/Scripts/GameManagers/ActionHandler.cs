using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler
{
    public static void HandleAction(string action) 
    {
        switch (action) 
        {
            case "love-fight":
                if (LoveLevelManager.current != null) LoveLevelManager.current.CompletePhaseAfterSeconds(2);
                break;

            case "dialogue-continue":
                if (LoveLevelManager.current != null) LoveLevelManager.current.CompletePhaseAfterSeconds(2);
                break;

            case "damage-player":
                DealPlayerDamage(1);
                break;

            case "kill-player":
                DealPlayerDamage(999);
                break;

            default:
                Debug.LogWarning("Action flag: " + action + " is not defined!");
                break;
        }
    }

    private static void DealPlayerDamage(int damageAmount) 
    {
        if (GameManager.current != null && GameManager.current.Player != null) GameManager.current.Player.TakeDamage(damageAmount);
    }
}
