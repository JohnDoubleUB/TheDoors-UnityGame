﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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
    }

    public void SaveGameMenu() 
    {

        if (UIManager.current != null)
        {
            UIManager.current.SetContextsActive(false, UIContextType.PauseMain, UIContextType.LoadMenu);
            UIManager.current.SetContextsActive(true, UIContextType.SaveMenu);
        }
    }

    public void LoadGameMenu()
    {

        if (UIManager.current != null)
        {
            UIManager.current.SetContextsActive(false, UIContextType.PauseMain, UIContextType.SaveMenu);
            UIManager.current.SetContextsActive(true, UIContextType.LoadMenu);
        }
    }

    public void SaveGame() 
    {
        Debug.Log("Save Game!");
    }

    public void LoadGame() 
    {
        Debug.Log("Load Game!");
    }

    public void MainPauseMenu() 
    {
        if (UIManager.current != null)
        {
            UIManager.current.SetContextsActive(false, UIContextType.SaveMenu, UIContextType.LoadMenu);
            UIManager.current.SetContextsActive(true, UIContextType.PauseMain);
        }
    }
}