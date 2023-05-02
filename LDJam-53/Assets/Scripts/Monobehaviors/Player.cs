using System;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event EventHandler<PlayerPropertyChangedNotificationArgs> PropertyChangedNotification;

    private int _cargoMax = 10;
    private int _currentCargo = 0;
    private int _currentHealth = 2;
    private int _maxHealth = 2;

    private float asteroidTimer = 0;
    private float _decel = 1f;
    private float _money = 0.00f;
    private float _thrust = 1f;

    private bool canTakeDamage = true;
    private bool _hasControl = true;
    private bool _isAlive = true;

    public AudioClip ErrorSound;
    public AudioClip ImpactHitSound;
    private AudioSource myAudioSource;

    public KeyCode RotLeft = KeyCode.Q;
    public KeyCode RotRight = KeyCode.E;
    public KeyCode ThrustF = KeyCode.W;
    public KeyCode ThrustR = KeyCode.S;
    public KeyCode BrakeRot = KeyCode.R;
    public KeyCode BrakeThr = KeyCode.F;
    public KeyCode CloseMenu = KeyCode.Escape;

    public LogisticsLocation LastStationVisited;

    public float AsteroidCollisionDelay = 1f;

    public float Thrust
    {
        get => this._thrust;
        set
        {
            this._thrust = value;
            this.OnPropertyChanged(this._thrust);
        }
    }

    public int CargoMax
    {
        get => this._cargoMax;
        set
        {
            this._cargoMax = value;
            this.OnPropertyChanged($"{this.CurrentCargo}/{this._cargoMax}", "Cargo");
        }
    }

    public int CurrentCargo
    {
        get => this._currentCargo;
        set
        {
            this._currentCargo = value;
            this.OnPropertyChanged($"{this._currentCargo}/{this.CargoMax}", "Cargo");
        }
    }

    public float Decel
    {
        get => this._decel;
        set
        {
            this._decel = value;
            this.OnPropertyChanged(this._decel);
        }
    }

    public bool IsAlive
    {
        get => this._isAlive;
        set
        {
            this._isAlive = value;
            this.OnPropertyChanged(this._isAlive);
        }
    }

    public int CurrentHealth
    {
        get => this._currentHealth;
        set
        {
            this._currentHealth = value;
            this.OnPropertyChanged(this._currentHealth);
        }
    }

    public int MaxHealth
    {
        get => this._maxHealth;
        set
        {
            this._maxHealth = value;
            this.OnPropertyChanged(this._maxHealth);
        }
    }

    public float Money
    {
        get => this._money;
        set
        {
            this._money = value;
            this.OnPropertyChanged(this._money);
        }
    }

    public bool HasControl
    {
        get => this._hasControl;
        set
        {
            this._hasControl = value;
            this.OnPropertyChanged(this._hasControl);
        }
    }

    private void Start()
    {
        this.myAudioSource = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PlaySceneManager.GameState == GameStateEnum.Running)

        this.asteroidTimer += Time.deltaTime;
        if (this.asteroidTimer >= this.AsteroidCollisionDelay)
        {
            this.canTakeDamage = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collisionData)
    {
        if (collisionData.gameObject.CompareTag("Asteroid"))
        {
            this.HandleAsteroidCollision(collisionData);
        }
    }

    private void HandleAsteroidCollision(Collision2D collisionData)
    {
        if (this.canTakeDamage)
        {
            this.CurrentHealth--;
            this.canTakeDamage = false;
            this.asteroidTimer = 0;
            this.PlayAudioClip(this.ImpactHitSound);
        }

        if (this.CurrentHealth <= 0)
        {
            this.IsAlive = false;
            this.HasControl = false;
        }
    }

    public void HandleUpgrade(string propertyName, float value, float cost)
    {
        if (this.Money - cost < 0)
        {
            this.myAudioSource.PlayOneShot(this.ErrorSound);
            return;
        }

        this.Money -= cost;
        switch (propertyName)
        {
            case nameof(this.CurrentHealth):
                this.CurrentHealth += (int)value;
                break;
            case nameof(this.MaxHealth):
                this.MaxHealth += (int)value;
                this.CurrentHealth = this.MaxHealth;
                break;
            case nameof(this.Thrust):
                this.Thrust += (float)value;
                break;
            case nameof(this.Decel):
                this.Decel += (float)value;
                break;
            case nameof(this.CargoMax):
                this.CargoMax += (int)value;
                break;
            default:
                Debug.Log("Unexpected upgrade type!");
                break;
        }
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (this.myAudioSource != null)
        {
            this.myAudioSource.PlayOneShot(clip);
        }
    }

    private void OnPropertyChanged(object value, [CallerMemberName] string propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return;
        }

        PropertyChangedNotification?.Invoke(null, new()
        {
            PropertyName = propertyName,
            NewValue = value,
        });
    }
}