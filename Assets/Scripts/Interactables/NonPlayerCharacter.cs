using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NonPlayerCharacter : Interactable
{

    public bool TriggerNPCDialogueOnInteract = true;
    public NPCDataObject NpcDataObject;

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

        if (TriggerNPCDialogueOnInteract) InitiateDialogue();
    }

    public void InitiateDialogue()
    {
        if (NpcDataObject != null && NpcDataObject.DialogueRootings.Any()) //Check we have a data object and that it has rootings
        {

            foreach (DialogueTreeRooting dialogueTreeRooting in NpcDataObject.DialogueRootings)
            {
                if (GameManager.current.HasFlag(dialogueTreeRooting.Name)) continue; //if this dialogue has already been had skip past it

                //Check we meet all the flag requirements
                bool meetsRequiredFlags = dialogueTreeRooting.RequiredFlags.Any() ? GameManager.current.HasAllFlags(dialogueTreeRooting.RequiredFlags) : true;
                bool meetsUnRequiredFlags = dialogueTreeRooting.UnRequiredFlags.Any() ? !GameManager.current.HasAnyFlags(dialogueTreeRooting.UnRequiredFlags) : true;

                if (meetsRequiredFlags && meetsUnRequiredFlags) //if this passes then we have found the currently required dialogue!
                {
                    DialogueManager.current.LoadDialogueTree(NpcDataObject.DialogueObjectName, dialogueTreeRooting.Name);
                    break;
                }
            }

        }
    }

}
