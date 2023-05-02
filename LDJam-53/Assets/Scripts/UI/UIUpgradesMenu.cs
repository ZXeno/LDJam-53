using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradesMenu : MonoBehaviour
{
    public AudioClip MenuOpenSound;
    public AudioClip MenuCloseSound;

    public GameObject ThrustUpgradeObject;
    public GameObject BrakingUpgradeObject;
    public GameObject CargoUpgradeButton;
    public GameObject HealthUpgradeButton;
    public GameObject CloseButton;

    public bool IsOpen => this.menuSprite.enabled;

    private Image menuSprite;
    private List<MenuItem> menuItems = new List<MenuItem>();
    private Player player;

    // Start is called before the first frame update
    public void Setup(Player player)
    {
        this.player = player;
        this.menuSprite = this.gameObject.GetComponent<Image>();

        this.menuItems.Add(new MenuItem()
        {
            Button = this.ThrustUpgradeObject.GetComponent<Button>(),
            Sprite = this.ThrustUpgradeObject.GetComponent<Image>(),
            UpgradeComponent = this.ThrustUpgradeObject.GetComponent<UIUpgradeButton>(),
        });

        this.menuItems.Add(new MenuItem()
        {
            Button = this.BrakingUpgradeObject.GetComponent<Button>(),
            Sprite = this.BrakingUpgradeObject.GetComponent<Image>(),
            UpgradeComponent = this.BrakingUpgradeObject.GetComponent<UIUpgradeButton>(),
        });

        this.menuItems.Add(new MenuItem()
        {
            Button = this.CargoUpgradeButton.GetComponent<Button>(),
            Sprite = this.CargoUpgradeButton.GetComponent<Image>(),
            UpgradeComponent = this.CargoUpgradeButton.GetComponent<UIUpgradeButton>(),
        });

        this.menuItems.Add(new MenuItem()
        {
            Button = this.HealthUpgradeButton.GetComponent<Button>(),
            Sprite = this.HealthUpgradeButton.GetComponent<Image>(),
            UpgradeComponent = this.HealthUpgradeButton.GetComponent<UIUpgradeButton>(),
        });

        // close button setup
        var closebtn = this.CloseButton.GetComponent<Button>();
        closebtn.onClick.AddListener(this.Hide);
        this.menuItems.Add(new MenuItem()
        {
            Button = this.CloseButton.GetComponent<Button>(),
            Sprite = this.CloseButton.GetComponent<Image>(),
            UpgradeComponent = null,
        });

        this.menuSprite.enabled = false;
        foreach (MenuItem item in this.menuItems)
        {
            item.Button.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        this.menuSprite.enabled = true;
        foreach (MenuItem item in this.menuItems)
        {
            item.Button.gameObject.SetActive(true);
            item.Button.enabled = true;
            item.Sprite.enabled = true;

            if (item.UpgradeComponent != null)
            {
                item.UpgradeComponent.player = this.player;
            }
        }

        this.player.PlayAudioClip(this.MenuOpenSound);
    }

    public void Hide()
    {
        this.menuSprite.enabled = false;
        foreach (MenuItem item in this.menuItems)
        {
            item.Button.gameObject.SetActive(false);
        }

        this.player.PlayAudioClip(this.MenuCloseSound);
    }

    private class MenuItem
    {
        public Button Button { get; set; }

        public Image Sprite { get; set; }

        public UIUpgradeButton UpgradeComponent { get; set; }
    }
}
