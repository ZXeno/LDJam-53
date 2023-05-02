using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaySceneManager : MonoBehaviour
{
    public static GameStateEnum GameState = GameStateEnum.Loading;
    public AudioClip Schwoop;

    // eh?
    public int RunTimeInSeconds = 60;

    public AudioClip[] MusicTracks;

    // Asteroids
    public int AsteroidSpawnCount = 20;
    public int StationSpawnCount = 10;
    public float AsteroidSpawnDistance = 5;
    private List<GameObject> SpawnedAsteroids = new List<GameObject>();
    public GameObject AsteroidContainer;
    public GameObject[] AsteroidPrefabs;

    // stations
    public GameObject[] StationPrefabs = new GameObject[] {};
    private List<LogisticsLocation> stations = new List<LogisticsLocation>();
    public Vector3 CenterPointOfAllStations;

    // Player
    public GameObject PlayerPrefab;
    private Player player;
    private PlayerController playerCtrlr;

    // this
    private static PlaySceneManager instance;

    // UI related
    public UIManager uiManager;


    // Programmatic Setup
    void Start()
    {
        // player
        GameObject player = Instantiate(this.PlayerPrefab);
        this.player = player.GetComponent<Player>();
        this.playerCtrlr = player.GetComponent<PlayerController>();

        // camera
        CameraController camera = Camera.main.GetComponent<CameraController>();
        camera.PlayerTransform = this.player.transform;

        // stations
        for (int x = 0; x < this.StationSpawnCount; x++)
        {
            GameObject newStation = Instantiate(
                this.StationPrefabs[Random.Range(0, this.StationPrefabs.Length)],
                this.transform);
            LogisticsLocation newStationComp = newStation.GetComponent<LogisticsLocation>();
            newStationComp.StationName = Names.GetRandomStationName();

            if (x == 0)
            {
                newStation.transform.position = new Vector3(
                    player.transform.position.x + 2,
                    player.transform.position.y,
                    0);
            }
            else
            {
                newStation.transform.position = this.GetNewStationLocation();
            }

            this.stations.Add(newStationComp);
        }

        // Asteroids
        for (int x = 0; x < this.AsteroidSpawnCount; x++)
        {
            GameObject randomPrefab = this.AsteroidPrefabs[Random.Range(0, this.AsteroidPrefabs.Length - 1)];
            GameObject child = Instantiate(randomPrefab, this.AsteroidContainer.transform);
            child.transform.position = this.GetRandomAsteroidLocation();
            child.name = $"Asteroid_{x}";
            this.SpawnedAsteroids.Add(child);
        }

        // ui
        this.uiManager = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<UIManager>();
        this.uiManager.Setup(this.player, this.stations);

        this.SavePlayerStartValues();

        // All setup is complete, set gamestate to runnign
        GameState = GameStateEnum.Running;
        instance = this;
    }

    public void Update()
    {
        if (!this.player.IsAlive)
        {
            GameState = GameStateEnum.PlayerDead;
        }
    }

    private Dictionary<string, object> playervalues = new Dictionary<string, object>();
    private void SavePlayerStartValues()
    {
        this.playervalues.Add(nameof(this.player.transform.position), this.player.transform.position);
        this.playervalues.Add(nameof(this.player.Thrust), this.player.Thrust);
        this.playervalues.Add(nameof(this.player.MaxHealth), this.player.MaxHealth);
        this.playervalues.Add(nameof(this.player.CargoMax), this.player.CargoMax);
        this.playervalues.Add(nameof(this.player.Decel), this.player.Decel);
    }

    public void ResetPlayer()
    {
        GameState = GameStateEnum.Loading;

        this.player.transform.position = (Vector3)this.playervalues[nameof(this.player.transform.position)];
        this.player.Thrust = (float)this.playervalues[nameof(this.player.Thrust)];
        this.player.CargoMax = (int)this.playervalues[nameof(this.player.CargoMax)];
        this.player.MaxHealth = (int)this.playervalues[nameof(this.player.MaxHealth)];
        this.player.Decel = (float)this.playervalues[nameof(this.player.Decel)];
        this.player.CurrentHealth = this.player.MaxHealth;
        this.player.CurrentCargo = 0;
        this.player.Money = 0;
        this.player.LastStationVisited = null;
        this.player.IsAlive = true;
        this.player.HasControl = true;
        this.player.PlayAudioClip(this.Schwoop);

        GameState = GameStateEnum.Running;
    }

    public static void SetPauseState(GameStateEnum state)
    {
        if (state != GameStateEnum.Running || state != GameStateEnum.Paused)
        {
            return;
        }

        if (GameState != GameStateEnum.PlayerDead
            && GameState != GameStateEnum.Loading)
        {
            GameState = state;
        }
    }

    public static void ResetPlayerState()
    {
        instance.ResetPlayer();
    }

    private Vector3 GetNewStationLocation()
    {
        float minSpawnDistance = this.playerCtrlr.MaxVelocity * this.RunTimeInSeconds / 2f;
        float maxSpawnDistance = this.playerCtrlr.MaxVelocity * (this.RunTimeInSeconds / 4f * 3f);

        Vector3 location = Random.onUnitSphere * Random.Range(minSpawnDistance, maxSpawnDistance);
        return new Vector3(location.x, location.y, 0);
    }

    private Vector3 GetRandomAsteroidLocation()
    {
        float minSpawnDistance = -this.AsteroidSpawnDistance;
        float maxSpawnDistance = this.GetRadiusOfFurthestStation() * 1.33f;

        Vector2 centerPoint = this.GetCenterPointOfStations();
        Vector2 candidate = Random.insideUnitCircle * Random.Range(minSpawnDistance, maxSpawnDistance) + centerPoint;
        while (this.IsTooCloseToStationOrPlayer(candidate))
        {
            candidate = Random.insideUnitCircle * Random.Range(minSpawnDistance, maxSpawnDistance) + centerPoint;
        }

        return new Vector3(
            candidate.x,
            candidate.y,
            0);
    }

    private Vector3 GetCenterPointOfStations()
    {
        Vector3 vectorSum = Vector3.zero;

        foreach (LogisticsLocation station in this.stations)
        {
            vectorSum += station.transform.position;
        }

        this.CenterPointOfAllStations = vectorSum / this.stations.Count;
        return this.CenterPointOfAllStations;
    }

    private float GetRadiusOfFurthestStation()
    {
        float furthestDistance = float.MinValue;
        foreach (LogisticsLocation station in this.stations)
        {
            float distance = Vector3.Distance(station.transform.position, this.CenterPointOfAllStations);
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
            }
        }

        return furthestDistance;
    }

    private bool IsTooCloseToStationOrPlayer(Vector3 candidate)
    {
        float distanceToPlayer = Vector3.Distance(candidate, this.playerCtrlr.transform.position);
        if (distanceToPlayer <= this.AsteroidSpawnDistance)
        {
            return true;
        }

        foreach (LogisticsLocation station in this.stations)
        {
            Vector3 position = station.transform.position;
            float distance = Vector3.Distance(position, candidate);
            if (distance <= this.AsteroidSpawnDistance)
            {
                return true;
            }
        }

        return false;
    }
}
