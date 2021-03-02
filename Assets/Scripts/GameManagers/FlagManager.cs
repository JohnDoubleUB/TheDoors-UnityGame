using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class FlagManager : MonoBehaviour
{
    public List<string> flags = new List<string>();

    public List<string> dialogueTreeFlags = new List<string>();

    public List<string> Flags { get { return flags; } }

    public List<string> DialogueTreeFlags { get { return dialogueTreeFlags; } }

    protected abstract void UpdateSessionFlags(string[] updatedFlags, FlagType flagType = FlagType.Progress);

    protected abstract void QueueActions(params string[] actions);

    protected void LoadFlags(List<string> activeFlags)
    {
        flags = activeFlags;
    }

    protected void LoadDialogueTreeFlags(List<string> activeDialogueTreeFlags) 
    {
        dialogueTreeFlags = activeDialogueTreeFlags;
    }

    public void AddFlag(string flag, FlagType flagType = FlagType.Progress)
    {
        switch (flagType) 
        {
            case FlagType.Progress:
                if (!flags.Contains(flag)) flags.Add(flag);
                break;

            case FlagType.DialogueName:
                if (!dialogueTreeFlags.Contains(flag)) dialogueTreeFlags.Add(flag);
                break;
        }

       //if (!flags.Contains(flag)) flags.Add(flag);

        UpdateSessionFlags(Flags.ToArray(), flagType);
    }

    public void AddActionFlag(string flag) 
    {
        AddFlag(flag);
        QueueActions(flag);
    }

    public void AddFlags(string[] flags, FlagType flagType = FlagType.Progress)
    {
        foreach (string flag in flags)
        {
            switch (flagType) 
            {
                case FlagType.Progress:
                    if (!this.flags.Contains(flag)) this.flags.Add(flag);
                    break;

                case FlagType.DialogueName:
                    if (!dialogueTreeFlags.Contains(flag)) dialogueTreeFlags.Add(flag);
                    break;
            }
            
            //if (!this.flags.Contains(flag)) this.flags.Add(flag);
        }

        UpdateSessionFlags(Flags.ToArray(), flagType);
    }

    public void AddActionFlags(string[] flags)
    {
        AddFlags(flags);
        QueueActions(flags);
    }

    public void RemoveFlag(string flag, FlagType flagType = FlagType.Progress)
    {
        switch (flagType) 
        {
            case FlagType.Progress:
                if (flags.Contains(flag)) flags.Remove(flag);
                break;

            case FlagType.DialogueName:
                if (dialogueTreeFlags.Contains(flag)) dialogueTreeFlags.Remove(flag);
                break;
        }
        //if (flags.Contains(flag)) flags.Remove(flag);

        UpdateSessionFlags(Flags.ToArray(), flagType);
    }

    public void RemoveFlags(string[] flags, FlagType flagType = FlagType.Progress)
    {
        foreach (string flag in flags) 
        {
            switch (flagType) 
            {
                case FlagType.Progress:
                    if (this.flags.Contains(flag)) this.flags.Remove(flag);
                    break;

                case FlagType.DialogueName:
                    if (dialogueTreeFlags.Contains(flag)) dialogueTreeFlags.Remove(flag);
                    break;
            }
            //if (this.flags.Contains(flag)) this.flags.Remove(flag);
        }
            

        UpdateSessionFlags(Flags.ToArray(), flagType);
    }

    public void ToggleFlag(string flag, FlagType flagType = FlagType.Progress)
    {
        switch (flagType) 
        {
            case FlagType.Progress:
                if (flags.Contains(flag)) flags.Remove(flag);
                else flags.Add(flag);
                break;
            
            case FlagType.DialogueName:
                if (dialogueTreeFlags.Contains(flag)) dialogueTreeFlags.Remove(flag);
                else dialogueTreeFlags.Add(flag);
                break;
        }
        //if (flags.Contains(flag)) flags.Remove(flag);
        //else flags.Add(flag);

        UpdateSessionFlags(Flags.ToArray(), flagType);
    }

    public bool HasFlag(string flag, FlagType flagType = FlagType.Progress)
    {
        switch (flagType) 
        {
            case FlagType.DialogueName:
                return dialogueTreeFlags.Contains(flag);

            case FlagType.Progress:
            default:
                return flags.Contains(flag);
        }
        //return flags.Contains(flag);
    }

    public bool[] HasFlags(string[] flags, FlagType flagType = FlagType.Progress)
    {
        return flags.Select(x => HasFlag(x, flagType)).ToArray();
    }

    public bool HasAllFlags(string[] flags, FlagType flagType = FlagType.Progress)
    {
        foreach (string flag in flags)
            if (!HasFlag(flag, flagType)) return false;

        return true;
    }

    public bool HasAnyFlags(string[] flags, FlagType flagType = FlagType.Progress)
    {
        foreach (string flag in flags)
            if (HasFlag(flag, flagType)) return true;

        return false;
    }
}

public enum FlagType 
{
    Progress,
    DialogueName
}
