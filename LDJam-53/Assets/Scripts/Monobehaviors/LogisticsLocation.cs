using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LogisticsLocation : MonoBehaviour
{
    public string StationName = string.Empty;
    public AudioClip RewardSfx;
    public AudioClip DockingSoundEffect;

    public float BaseRewardPerUnit = 15f;
    public float MaxRewardPerUnit = 30f;
    public float CurrentlyAvailableReward => this.CalculateReward();
    public float MinDistance = 10f;
    public float MaxDistance = 100f;
    public float DistanceWeight = 0.2f; // A percentage value between 0 and 1


    public int MaxAvailableGoods = 200;
    public int currentAvailableGoods = 0;

    private int currentReceivedGoods = 0;
    private int maxReceivedGoods = 200;

    private float goodsMinDepletionRate = 20f;
    private float goodsMinAccretionRate = 20f;
    private float goodsMaxDepletionRate = 5f;
    private float goodsMaxAccretionRate = 5f;
    private float goodsDepletionRate = 1f;
    private float goodsAccretionRate = 1f;
    private float depletiontimer = 0;
    private float accretiontimer = 0;


    public GameObject NavigationPoint;
    public GameObject PlayerObject;
    public Player Player;
    public PlayerController PlayerController;
    public float ArrivedThreashold = .0025f;
    public float ReleaseForce = 1f;

    private bool havePlayer =>
        this.PlayerObject != null
        && this.PlayerController != null
        && this.Player != null;

    private bool rewardProcessed = false;

    public float DockingDelay = 1;
    private float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        this.currentAvailableGoods = Random.Range(20, this.MaxAvailableGoods);
        this.goodsDepletionRate = Random.Range(this.goodsMaxDepletionRate, this.goodsMinDepletionRate);
        this.goodsAccretionRate = Random.Range(this.goodsMaxAccretionRate, this.goodsMinAccretionRate);
    }

    // Update is called once per frame
    void Update()
    {
        this.ProcessStationCargo();

        if (!this.havePlayer)
        {
            return;
        }

        if (!this.MovePlayerToDockPoint())
        {
            return;
        }

        this.timer += Time.deltaTime;
        if (this.timer <= this.DockingDelay / 2 && !this.rewardProcessed)
        {
            this.DispenseReward();
            return;
        }

        if (this.timer <= this.DockingDelay)
        {
            return;
        }

        this.LoadPlayerCargo();

        this.timer = 0;
        this.ReleasePlayer();
    }

    private void LoadPlayerCargo()
    {
        if (this.currentAvailableGoods <= 0)
        {
            return;
        }

        int estimatedRemainingGoods = this.currentAvailableGoods - this.Player.CargoMax;
        if (estimatedRemainingGoods < 0 && this.currentAvailableGoods > 0)
        {
            this.Player.CurrentCargo = this.currentAvailableGoods;
            this.currentAvailableGoods = 0;
            return;
        }

        if (estimatedRemainingGoods > 0)
        {
            this.Player.CurrentCargo = this.Player.CargoMax;
            this.currentAvailableGoods -= this.Player.CargoMax;
        }
    }

    private void ProcessStationCargo()
    {
        this.depletiontimer += Time.deltaTime;
        this.accretiontimer += Time.deltaTime;

        if (this.accretiontimer >= this.goodsAccretionRate)
        {
            this.currentAvailableGoods = Mathf.Clamp(this.currentAvailableGoods++, 0, this.MaxAvailableGoods);
            this.accretiontimer = 0;
        }

        if (this.depletiontimer >= this.goodsDepletionRate)
        {
            this.currentReceivedGoods = Mathf.Clamp(this.currentReceivedGoods--, 0, this.maxReceivedGoods);
            this.depletiontimer = 0;
        }
    }

    private bool MovePlayerToDockPoint()
    {
        Transform pTransform = this.PlayerObject.transform;
        Vector3 navPosition = this.NavigationPoint.transform.position;
        float distanceFromArrival = Vector3.Distance(pTransform.position, navPosition);
        if (distanceFromArrival >= this.ArrivedThreashold)
        {
            Vector3 newPlayerPos = new Vector3(
                Mathf.Lerp(pTransform.position.x, navPosition.x, Time.deltaTime),
                Mathf.Lerp(pTransform.position.y, navPosition.y, Time.deltaTime),
                0);

            pTransform.position = newPlayerPos;
            return false;
        }

        return true;
    }

    private void DispenseReward()
    {
        this.Player.PlayAudioClip(this.RewardSfx);
        this.rewardProcessed = true;

        if (this.Player.CurrentCargo > 0)
        {
            float distance = Vector3.Distance(
                this.transform.position,
                this.Player.LastStationVisited.transform.position);

            float calculatedBaseReward = this.CalculateReward();
            float calculatedReward = this.ApplyDistanceWeight(calculatedBaseReward, distance);
            calculatedReward *= this.Player.CurrentCargo;
            this.Player.Money += calculatedReward;
        }

        this.currentReceivedGoods = Mathf.Clamp(
            this.Player.CurrentCargo + this.currentReceivedGoods,
            0, this.maxReceivedGoods);

        this.Player.CurrentCargo = 0;
    }

    private void ReleasePlayer()
    {
        this.ApplyReleaseForce();
        this.Player.HasControl = true;
        this.Player.LastStationVisited = this;

        this.Player = null;
        this.PlayerController = null;
        this.PlayerObject = null;
        this.rewardProcessed = false;
    }

    private void ApplyReleaseForce()
    {
        Vector3 direction = this.PlayerController.transform.position - this.transform.position;
        this.PlayerController.Velocity = new Vector2(
            direction.x * this.ReleaseForce,
            direction.y * this.ReleaseForce);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = new Vector3(1000, 1000, 1000);
            return;
        }

        this.PlayerObject = other.gameObject;
        this.Player = other.gameObject.GetComponent<Player>();
        this.PlayerController = other.gameObject.GetComponent<PlayerController>();
        this.Player.HasControl = false;
        this.PlayerController.Velocity = Vector2.zero;
        this.PlayerController.AngularVelocity = 0f;
        this.Player.PlayAudioClip(this.DockingSoundEffect);
    }

    private float ApplyDistanceWeight(float initialReward, float distance)
    {
        distance = Mathf.Clamp(distance, this.MinDistance, this.MaxDistance);
        float distanceFactor = (distance - this.MinDistance) / (this.MaxDistance - this.MinDistance);
        float weightedReward = initialReward * (1 + (this.DistanceWeight * distanceFactor));

        return weightedReward;
    }

    private float CalculateReward()
    {
        float lerpVal = 1 - this.currentReceivedGoods / (float)this.maxReceivedGoods;
        float rawRewardValue = Mathf.Lerp(this.BaseRewardPerUnit, this.MaxRewardPerUnit, lerpVal);
        return (float)Math.Round(rawRewardValue, 2);
    }
}
