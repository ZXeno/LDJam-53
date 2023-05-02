using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeButton : MonoBehaviour
{
    public AudioClip BuyUpgradeSfx;
    public Player player;
    public string PropertyName = "PropertyName";
    public float UpgradeAmount = 1;
    public float Cost = 1000f;

    private Button button;

    private void Start()
    {
        this.button = this.GetComponent<Button>();
        this.button.onClick.AddListener(this.OnClick);
    }

    public void OnClick()
    {
        this.player.HandleUpgrade(this.PropertyName, this.UpgradeAmount, this.Cost);
        this.player.PlayAudioClip(this.BuyUpgradeSfx);
    }
}
