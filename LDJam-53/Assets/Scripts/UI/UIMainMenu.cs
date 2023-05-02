using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public Button StartGameButton;
    public Button ExitButton;

    private void Start()
    {
        this.StartGameButton.onClick.AddListener(this.HandleStartClick);
        this.ExitButton.onClick.AddListener(this.HandleExitClick);
    }

    public void HandleStartClick()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void HandleExitClick()
    {
        Application.Quit();
    }
}
