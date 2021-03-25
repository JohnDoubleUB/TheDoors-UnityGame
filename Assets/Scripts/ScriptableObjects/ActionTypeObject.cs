using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionTypeDefinitions", menuName = "ScriptableObjects/ActionTypeObject")]
public class ActionTypeObject : ScriptableObject
{
    public string[] InstantActions;
}
