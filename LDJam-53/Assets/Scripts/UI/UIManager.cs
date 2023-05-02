using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject EdgeIndicatorPrefab;
    public UITextDisplayElement MoneyDisplayElement;
    public UITextDisplayElement ThrustDisplayElement;
    public UITextDisplayElement BrakingDisplayElement;
    public UITextDisplayElement CargoDisplayElement;

    public UIHealthBar HealthBar;
    private List<UIEdgeIndicator> edgeIndicators = new List<UIEdgeIndicator>();
    private Player player;
    private List<LogisticsLocation> stations;
    public UIPauseMenu PauseMenu;

    public UIUpgradesMenu upgradesMenu;
    public UIOpenUpgradesMenuButton openUpgradesMenuButton;

    public void Setup(Player player, List<LogisticsLocation> stations)
    {
        this.player = player;
        this.stations = stations;

        if (this.HealthBar == null
            || this.MoneyDisplayElement == null
            || this.BrakingDisplayElement == null
            || this.ThrustDisplayElement == null
            || this.CargoDisplayElement == null)
        {
            this.GetUiElements();
        }

        this.CreateScreenEdgeIndicators();
        this.upgradesMenu.Setup(this.player);
        this.openUpgradesMenuButton.Setup(this.player, this.upgradesMenu);
        this.HealthBar.Setup(this.player);
    }

    private GameStateEnum previousGameState = GameStateEnum.Loading;
    private void Update()
    {
        if (Input.GetKeyDown(this.player.CloseMenu)
            && PlaySceneManager.GameState != GameStateEnum.PlayerDead)
        {
            if (this.upgradesMenu.IsOpen)
            {
                this.upgradesMenu.Hide();
                this.previousGameState = PlaySceneManager.GameState;
                return;
            }

            if (!this.PauseMenu.IsOpen)
            {
                this.PauseMenu.Show();
            }
        }

        if (PlaySceneManager.GameState != this.previousGameState
            && PlaySceneManager.GameState == GameStateEnum.PlayerDead)
        {
            this.PauseMenu.Show(true);
        }

        this.previousGameState = PlaySceneManager.GameState;
    }

    private void GetUiElements()
    {
        if (this.HealthBar == null)
        {
            this.HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<UIHealthBar>();
        }

        if (this.MoneyDisplayElement == null)
        {
            this.MoneyDisplayElement = GameObject.FindGameObjectWithTag("MoolahDisp").GetComponent<UITextDisplayElement>();
            this.MoneyDisplayElement.SetText(this.player.Money.ToString(this.MoneyDisplayElement.NumberFormat) ?? "0.00");
        }

        if (this.CargoDisplayElement == null)
        {
            this.CargoDisplayElement = GameObject.FindGameObjectWithTag("CargoDisp").GetComponent<UITextDisplayElement>();
            this.CargoDisplayElement.SetText($"0 / {this.player.CargoMax.ToString(this.CargoDisplayElement.NumberFormat)}" ?? "0");
        }

        if (this.BrakingDisplayElement == null)
        {
            this.BrakingDisplayElement = GameObject.FindGameObjectWithTag("BrakingDisp").GetComponent<UITextDisplayElement>();
            this.BrakingDisplayElement.SetText(this.player.Decel.ToString(this.BrakingDisplayElement.NumberFormat) ?? "0");
        }

        if (this.ThrustDisplayElement == null)
        {
            this.ThrustDisplayElement = GameObject.FindGameObjectWithTag("ThrustDisp").GetComponent<UITextDisplayElement>();
            this.ThrustDisplayElement.SetText(this.player.Thrust.ToString(this.ThrustDisplayElement.NumberFormat) ?? "0");
        }
    }

    private void CreateScreenEdgeIndicators()
    {
        GameObject uiroot = GameObject.FindGameObjectWithTag("UIRoot");
        foreach (LogisticsLocation station in this.stations)
        {
            UIEdgeIndicator indicator = Instantiate(this.EdgeIndicatorPrefab, uiroot.transform).GetComponent<UIEdgeIndicator>();
            indicator.Player = this.player.transform;
            indicator.Target = station.gameObject.transform;
        }
    }

    public void PlayerStateReset()
    {
        this.PauseMenu.Hide();
        this.upgradesMenu.Hide();
    }
}
