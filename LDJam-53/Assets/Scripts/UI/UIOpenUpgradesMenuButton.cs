using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class UIOpenUpgradesMenuButton : MonoBehaviour
{
    public Player player;
    public float DistanceVisible = 5f;
    public Image ImageSprite;
    public Button Button;

    private UIUpgradesMenu upgradesMenu;

    public bool IsVIsible => this.ImageSprite.enabled;

    public void Setup(Player player, UIUpgradesMenu upgradesMenu)
    {
        this.player = player;
        this.upgradesMenu = upgradesMenu;
        this.ImageSprite = this.gameObject.GetComponent<Image>();
        this.Button = this.gameObject.GetComponent<Button>();

        this.Button.onClick.AddListener(this.HandleClick);
    }

    private void HandleClick()
    {
        this.upgradesMenu.Show();
    }

    // Update is called once per frame
    void Update()
    {
        this.HandleVisibility();
        this.PositionButton();
    }

    private void PositionButton()
    {
        if (!this.IsVIsible
            || this.player.LastStationVisited == null)
        {
            return;
        }

        Vector3 proposedPosition = Camera.main.WorldToScreenPoint(this.player.LastStationVisited.transform.position);
        proposedPosition.x += 64;

        this.transform.position = proposedPosition;
    }

    private void HandleVisibility()
    {
        if (this.player?.LastStationVisited != null
            && Utils.WorldPositionIsInViewport(this.player.LastStationVisited.transform.position)
            && this.IsInVisibleDistance()
            && !this.upgradesMenu.IsOpen)
        {
            this.ImageSprite.enabled = true;
            this.Button.enabled = true;
            return;
        }

        this.ImageSprite.enabled = false;
        this.Button.enabled = false;
    }

    private void GetUiComponents()
    {
        this.ImageSprite = this.gameObject.GetComponent<Image>();
        this.Button = this.gameObject.GetComponent<Button>();

        this.Button.onClick.AddListener(this.HandleClick);
    }

    private bool IsInVisibleDistance()
    {
        float distance = Vector3.Distance(
            this.player.transform.position,
            this.player.LastStationVisited.transform.position);

        return distance <= this.DistanceVisible;
    }
}
