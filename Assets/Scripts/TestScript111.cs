using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript111 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveSystem.currentTextTest = "This was set in the main menu!";

        Debug.Log("Savesystem text set to: " + SaveSystem.currentTextTest);
    }
}
