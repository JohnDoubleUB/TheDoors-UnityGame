﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void StartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() 
    {
        Debug.Log("This will quit in the compiled game but doesn't work in the engine!");
        Application.Quit();
    }

    public void ReturnToGame()
    {
        if (UIManager.current != null) UIManager.current.SetContextsActive(false, UIContextType.PauseMenu);
        if (GameManager.current != null) GameManager.current.SetSelectedSaveOption(0);
    }

    public void SaveGameMenu() 
    {

        if (UIManager.current != null)
        {
            UIManager.current.SetContextsActive(false, UIContextType.PauseMain, UIContextType.LoadMenu);
            UIManager.current.SetContextsActive(true, UIContextType.SaveMenu, UIContextType.SaveSelection);
            if (GameManager.current != null) GameManager.current.RefreshSaveSlotData();
        }
    }

    public void LoadGameMenu()
    {

        if (UIManager.current != null)
        {
            UIManager.current.SetContextsActive(false, UIContextType.PauseMain, UIContextType.SaveMenu);
            UIManager.current.SetContextsActive(true, UIContextType.LoadMenu, UIContextType.SaveSelection);
            if (GameManager.current != null) GameManager.current.RefreshSaveSlotData();
        }
    }

    public void SaveGame() 
    {
        if (GameManager.current != null) GameManager.current.InitiateSave();
    }

    public void LoadGame() 
    {
        if (GameManager.current != null) GameManager.current.InitiateLoad();
    }

    public void MainPauseMenu() 
    {
        if (UIManager.current != null)
        {
            UIManager.current.SetContextsActive(false, UIContextType.SaveMenu, UIContextType.LoadMenu, UIContextType.SaveSelection);
            UIManager.current.SetContextsActive(true, UIContextType.PauseMain);
        }
    }
}