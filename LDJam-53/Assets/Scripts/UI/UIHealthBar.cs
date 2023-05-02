using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    private Player _player;
    public Player Player
    {
        get => this._player;
        set
        {
            if (value == null)
            {
                return;
            }

            if (value != this._player && this._player != null)
            {
                Player.PropertyChangedNotification -= this.HandleHealthChangeEvent;
            }

            this._player = value;
            if (this._player == null)
            {
                return;
            }

            Player.PropertyChangedNotification += this.HandleHealthChangeEvent;
            this.SetSprites();
        }
    }

    public GameObject HealthSpritePrefab;
    public GameObject EmptyHealthSpritePrefab;
    private List<Image> sprites = new List<Image>();

    private ObjectPool<Image> healthSpritePool;
    private ObjectPool<Image> emptySpritePool;


    public void Setup(Player player)
    {
        this.healthSpritePool =
            new ObjectPool<Image>(
                this.HealthSpritePrefab.GetComponent<Image>(),
                8,
                true,
                this.gameObject.transform);

        this.emptySpritePool =
            new ObjectPool<Image>(
                this.EmptyHealthSpritePrefab.GetComponent<Image>(),
                8,
                true,
                this.gameObject.transform);

        this.Player = player;
    }

    private void SetSprites()
    {
        foreach (Image sprite in this.sprites)
        {
            sprite.enabled = false;
            if (sprite.CompareTag("FilledHealthSprite"))
            {
                this.healthSpritePool.ReturnPooledObject(sprite);
                continue;
            }

            this.emptySpritePool.ReturnPooledObject(sprite);
        }

        this.sprites.Clear();

        for (int x = 0; x < this.Player.MaxHealth; x++)
        {
            Image spriteObject = null;
            if (x >= this.Player.CurrentHealth)
            {
                spriteObject = this.emptySpritePool.GetPooledObject();
            }
            else
            {
                spriteObject = this.healthSpritePool.GetPooledObject();
            }
            spriteObject.gameObject.SetActive(true);

            Vector3 calculatedPos = new Vector3(
                x * 42 + 64,
                64,
                0
            );

            spriteObject.enabled = true;
            spriteObject.gameObject.transform.position = calculatedPos;

            this.sprites.Add(spriteObject);
        }
    }

    private void HandleHealthChangeEvent(object sender, PlayerPropertyChangedNotificationArgs e)
    {
        if (e.PropertyName != "CurrentHealth")
        {
            return;
        }

        this.SetSprites();
    }

    private void OnDestroy()
    {
        Player.PropertyChangedNotification -= this.HandleHealthChangeEvent;
    }
}
