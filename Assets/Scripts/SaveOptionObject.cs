using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveOptionObject : MonoBehaviour
{
    public int saveNumber = 1;
    public Text saveNumberText;
    public Text saveFileContents;
    public Text selectedTextIndicator;

    public bool IsSelected 
    {
        get 
        {
            return selectedTextIndicator != null ? selectedTextIndicator.enabled : false;
        }
        set 
        {
            if (selectedTextIndicator != null) selectedTextIndicator.enabled = value;
        }
    }

    public void SaveFileSelected()
    {
        if (GameManager.current != null) GameManager.current.SetSelectedSaveOption(saveNumber);
    }

    private void Awake()
    {
        if (saveNumberText != null) saveNumberText.text = saveNumber.ToString();
        IsSelected = false;
    }

    private void Start()
    {
        if (GameManager.current != null) GameManager.current.AddSaveOptionObject(this); //TODO: This only happens when first enabled :( doensn't work first time fix
        Debug.Log("Start!");
    }

    public void SetContent(string content = null) 
    {
        if (string.IsNullOrEmpty(content))
        {
            saveFileContents.text = "Empty";
        }
        else 
        {
            saveFileContents.text = content;
        }
    }
}
