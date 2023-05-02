using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VelocityIndicator : MonoBehaviour
{
    private PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        this.controller = this.gameObject.GetComponentInParent<PlayerController>();
        if (this.controller == null)
        {
            Debug.LogError("COULD NOT FIND CONTROLLER IN VELOCITY INDICATOR SCRIPT");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = this.controller.Velocity;
        Vector3 ctrlPos = this.controller.transform.position;


        this.transform.position = new Vector3(
            ctrlPos.x + velocity.x,
            ctrlPos.y + velocity.y,
            this.transform.position.z);
    }
}
