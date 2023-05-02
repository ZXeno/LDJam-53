using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    public Button ExitButton;
    public Button ResumeButton;
    public GameObject TextObj;

    public Image MenuImg;

    public bool IsOpen => this.MenuImg.enabled;

    private void Start()
    {
        this.MenuImg = this.GetComponent<Image>();
        this.ExitButton.onClick.AddListener(this.HandleExitClicked);
        this.ResumeButton.onClick.AddListener(this.HandleResumeClicked);
    }

    public void Show(bool showPlayerDiedText = false)
    {
        if (showPlayerDiedText)
        {
            this.TextObj.SetActive(true);
        }

        this.MenuImg.enabled = true;
        this.ExitButton.gameObject.SetActive(true);
        this.ResumeButton.gameObject.SetActive(true);
        PlaySceneManager.SetPauseState(GameStateEnum.Paused);
    }

    public void Hide()
    {
        this.MenuImg.enabled = false;
        this.TextObj.SetActive(false);
        this.ExitButton.gameObject.SetActive(false);
        this.ResumeButton.gameObject.SetActive(false);
        PlaySceneManager.SetPauseState(GameStateEnum.Running);
    }

    public void HandleExitClicked()
    {
        Debug.Log("Exit Clicked");
        Application.Quit();
    }

    public void HandleResumeClicked()
    {
        this.Hide();

        if (PlaySceneManager.GameState == GameStateEnum.PlayerDead)
        {
            PlaySceneManager.ResetPlayerState();
        }
    }
}
