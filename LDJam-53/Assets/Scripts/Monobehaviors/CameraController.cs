using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float CameraZ = -10;
    public float CameraLag = 1;
    public Transform PlayerTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (this.PlayerTransform != null)
        {
            this.transform.position =
                new Vector3(
                    this.PlayerTransform.position.x,
                    this.PlayerTransform.position.y,
                    CameraZ);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        Vector3 ppos = this.PlayerTransform.position;
        this.transform.position =
            new Vector3(
                Mathf.Lerp(ppos.x, this.transform.position.x, Time.deltaTime * this.CameraLag),
                Mathf.Lerp(ppos.y, this.transform.position.y, Time.deltaTime * this.CameraLag),
                CameraZ);
    }
}
