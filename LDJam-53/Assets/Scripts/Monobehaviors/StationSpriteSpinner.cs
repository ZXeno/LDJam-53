using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class StationSpriteSpinner : MonoBehaviour
{
    public float AngularVelocity = 1;

    // Start is called before the first frame update
    void Start()
    {
        int direction = Random.Range(0, 2);
        this.AngularVelocity = direction == 1 ? this.AngularVelocity * -1 : this.AngularVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlaySceneManager.GameState != GameStateEnum.Running)
        {
            return;
        }

        Vector3 spin = new Vector3(0, 0, this.AngularVelocity * Time.deltaTime);
        this.transform.Rotate(spin);
    }
}
