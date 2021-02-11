using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDataObject", menuName = "ScriptableObjects/NPCDataObject")]
public class NPCDataObject : ScriptableObject 
{
    //I REALLY don't want to update this too often because when you change the shape of scriptable objects all your saved scriptable objects get ruined, 
    //Hence why our dialogue system reads xml and isn't entered in unity! Scriptiable objects, while cool are also evil

    public string DialogueObjectName;

    public List<DialogueTreeRooting> DialogueRootings;
}

[Serializable]
public class DialogueTreeRooting
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public string[] RequiredFlags;
    [SerializeField]
    public string[] UnRequiredFlags;
}
