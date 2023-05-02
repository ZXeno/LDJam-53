using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class AsteroidBehavior : MonoBehaviour
{
    public float RotationSpeed = 8f;
    public float AngularVelocity;
    public float RotationRateRange = 1f;
    public float EnableRange = 15f;

    public float DistanceFromPlayer = 0;
    private GameObject player;
    private PolygonCollider2D myCollider;

    private void Start()
    {
        this.AngularVelocity = Random.Range(-this.RotationRateRange, this.RotationRateRange);
        this.player = GameObject.FindGameObjectWithTag("Player");
        this.myCollider = this.GetComponent<PolygonCollider2D>();
        this.myCollider.enabled = false;
    }

    private void Update()
    {
        if (PlaySceneManager.GameState != GameStateEnum.Running)
        {
            return;
        }

        this.DistanceFromPlayer = Vector3.Distance(this.transform.position, this.player.transform.position);
        if (this.DistanceFromPlayer <= this.EnableRange && !this.myCollider.enabled)
        {
            this.myCollider.enabled = true;
        }
        else if (this.myCollider.enabled && this.DistanceFromPlayer > this.EnableRange)
        {
            this.myCollider.enabled = false;
        }
    }

    private void LateUpdate()
    {
        this.transform.Rotate(new Vector3(0,0,this.AngularVelocity));
    }
}
