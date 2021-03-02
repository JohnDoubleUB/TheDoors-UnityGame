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

    protected abstract void UpdateSessionFlags(string[] updatedFlags);

    protected abstract void QueueActions(params string[] actions);

    protected void LoadFlags(List<string> activeFlags)
    {
        flags = activeFlags;
    }

    protected void LoadDialogueTreeFlags(List<string> activeDialogueTreeFlags) 
    {
        dialogueTreeFlags = activeDialogueTreeFlags;
    }

    public void AddFlag(string flag)
    {
        if (!flags.Contains(flag)) flags.Add(flag);

        UpdateSessionFlags(Flags.ToArray());
    }

    public void AddActionFlag(string flag) 
    {
        AddFlag(flag);
        QueueActions(flag);
    }

    public void AddFlags(string[] flags)
    {
        foreach (string flag in flags)
        {
            if (!this.flags.Contains(flag)) this.flags.Add(flag);
        }

        UpdateSessionFlags(Flags.ToArray());
    }

    public void AddActionFlags(string[] flags)
    {
        AddFlags(flags);
        QueueActions(flags);
    }

    public void RemoveFlag(string flag)
    {
        if (flags.Contains(flag)) flags.Remove(flag);

        UpdateSessionFlags(Flags.ToArray());
    }

    public void RemoveFlags(string[] flags)
    {
        foreach (string flag in flags)
            if (this.flags.Contains(flag)) this.flags.Remove(flag);

        UpdateSessionFlags(Flags.ToArray());
    }

    public void ToggleFlag(string flag)
    {
        if (flags.Contains(flag)) flags.Remove(flag);
        else flags.Add(flag);

        UpdateSessionFlags(Flags.ToArray());
    }

    public bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    public bool[] HasFlags(string[] flags)
    {
        return flags.Select(x => HasFlag(x)).ToArray();
    }

    public bool HasAllFlags(string[] flags)
    {
        foreach (string flag in flags)
            if (!HasFlag(flag)) return false;

        return true;
    }

    public bool HasAnyFlags(string[] flags)
    {
        foreach (string flag in flags)
            if (HasFlag(flag)) return true;

        return false;
    }

}
