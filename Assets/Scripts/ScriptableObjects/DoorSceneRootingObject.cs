using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "DoorSceneRooting", menuName = "ScriptableObjects/DoorSceneRootingObject")]
public class DoorSceneRootingObject : ScriptableObject
{
    public List<DoorSceneRooting> doorSceneRootings;
}

[Serializable]
public class DoorSceneRooting 
{
    [SerializeField]
    public DoorName doorName;
    [SerializeField]
    public string sceneName;
}
