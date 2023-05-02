
using DefaultNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem LeftRetroEmitter;
    public ParticleSystem RightRetroEmitter;
    public ParticleSystem LeftEmitter;
    public ParticleSystem RightEmitter;
    public ParticleSystem MainEmitter;

    public Vector2 Velocity;
    public float AngularVelocity;
    public float Thrust = 10f;
    public float MaxVelocity = 50f;
    public float Decel = 10f;
    public float RotationalThrust = 5f;
    public float MaxRotationSpeed = 10f;
    public float AsteroidRepulsionForce = 2f;

    private Player Player;
    private float floatingPointThreshold = .003f;

    void Start()
    {
        this.Player = this.gameObject.GetComponent<Player>();

        // just get the damned emitters
        ParticleSystem[] emitterComponents = this.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem c in emitterComponents)
        {
            switch (c.name)
            {
                case nameof(this.MainEmitter):
                    this.MainEmitter = c;
                    break;
                case nameof(this.LeftEmitter):
                    this.LeftEmitter = c;
                    break;
                case nameof(this.RightEmitter):
                    this.RightEmitter = c;
                    break;
                case nameof(this.LeftRetroEmitter):
                    this.LeftRetroEmitter = c;
                    break;
                case nameof(this.RightRetroEmitter):
                    this.RightRetroEmitter = c;
                    break;
                default:
                    continue;
            }
        }

        this.HandleParticleEmitterStates(Vector2.zero);
    }

    private void Update()
    {
        if (PlaySceneManager.GameState != GameStateEnum.Running)
        {
            return;
        }

        this.Thrust = this.Player.Thrust;
        this.Decel = this.Player.Decel;

        this.CalculateAdjustmentsFromInput();
        this.transform.Rotate(new Vector3(0,0,this.AngularVelocity));

        this.transform.SetPositionAndRotation(
            new Vector3(
                this.transform.position.x + this.Velocity.x * Time.deltaTime,
                this.transform.position.y + this.Velocity.y * Time.deltaTime),
            this.transform.rotation);
    }

    private void CalculateAdjustmentsFromInput()
    {
        // get velocity adjustments from input
        float angularChange = this.CalculateRotationChange();
        Vector2 thrustChange = this.CalculateThrustValue();

        // calculate rotational change
        if (angularChange == 0
            && !this.AngularVelocity.WithinThreshold(this.floatingPointThreshold)
            && !this.RotationKeyPressed())
        {
            angularChange = this.CalculateAngularDrag();
        }
        else if (angularChange == 0 && this.AngularVelocity.WithinThreshold(this.floatingPointThreshold))
        {
            this.AngularVelocity = 0;
        }

        // apply angular change
        this.AngularVelocity = Mathf.Clamp(angularChange + this.AngularVelocity, -this.MaxRotationSpeed, this.MaxRotationSpeed);

        // calculate thruster change
        if (thrustChange == Vector2.zero
            && (!this.Velocity.x.WithinThreshold(this.floatingPointThreshold)
                || !this.Velocity.y.WithinThreshold(this.floatingPointThreshold))
            && !this.ThrusterKeyPressed())
        {
            thrustChange = this.CalculateVelocityDrag();
            this.Velocity += thrustChange;
        }
        else if (thrustChange != Vector2.zero)
        {
            Vector2 worldThrustChange = this.transform.TransformVector(thrustChange);

            this.Velocity = new Vector2(
                Mathf.Clamp(this.Velocity.x + worldThrustChange.x, -this.MaxVelocity, this.MaxVelocity),
                Mathf.Clamp(this.Velocity.y + worldThrustChange.y, -this.MaxVelocity, this.MaxVelocity));
        }
        else if (thrustChange == Vector2.zero
                 && this.Velocity.x.WithinThreshold(this.floatingPointThreshold)
                 && this.Velocity.y.WithinThreshold(this.floatingPointThreshold)
                 && !this.ThrusterKeyPressed())
        {
            this.Velocity = Vector2.zero;
        }

        if (!Input.GetKey(this.Player.BrakeThr))
        {
            // handle emitter change
            this.HandleParticleEmitterStates(thrustChange);
            return;
        }

        Vector2 normalizedVelocity = this.Velocity.normalized;
        float deceleration = this.Decel * Time.deltaTime;
        float brakingX = this.Velocity.x < 0 ?
            deceleration * normalizedVelocity.x
            :  -(deceleration * normalizedVelocity.x);

        float brakingY = this.Velocity.y < 0 ?
            deceleration * normalizedVelocity.y
            : -(deceleration * normalizedVelocity.y);

        if (this.Velocity.x.WithinThreshold(this.floatingPointThreshold))
        {
            brakingX = 0f;
        }

        if (this.Velocity.y.WithinThreshold(this.floatingPointThreshold))
        {
            brakingY = 0f;
        }

        thrustChange.x = brakingX;
        thrustChange.y = brakingY;

        // handle emitter change
        this.HandleParticleEmitterStates(thrustChange);

        this.Velocity += thrustChange;
    }

    private Vector2 CalculateVelocityDrag()
    {
        float adjustedVelocityX =
            this.Velocity.x.WithinThreshold(this.floatingPointThreshold) ?
                0 : this.CalcDragForValue(this.Velocity.x, 1);

        float adjustedVelocityY =
            this.Velocity.y.WithinThreshold(this.floatingPointThreshold) ?
                0 : this.CalcDragForValue(this.Velocity.y, 1);

        return new Vector2(adjustedVelocityX, adjustedVelocityY);
    }

    private float CalculateAngularDrag()
    {
        return CalcDragForValue(this.AngularVelocity, 2);
    }

    private float CalcDragForValue(float velocityValue, float divisor)
    {
        float adjustment = -(this.Decel / divisor * Time.deltaTime);
        adjustment = velocityValue < 0 ? -adjustment : adjustment;
        return adjustment;
    }

    private float CalculateRotationChange()
    {
        if (!this.Player.HasControl)
        {
            return 0;
        }

        float angularChange = 0;
        if (Input.GetKey(KeyCode.R))
        {
            angularChange = this.Decel * Time.deltaTime * -1;

            if (this.AngularVelocity < 0)
            {
                angularChange = -angularChange;
            }

            if (Mathf.Abs(this.AngularVelocity) < Mathf.Abs(angularChange))
            {
                angularChange = -this.AngularVelocity;
            }

            float newBrakingTarget = Mathf.Clamp(this.AngularVelocity + -angularChange, -this.MaxRotationSpeed, this.MaxRotationSpeed);

            if (Mathf.Abs(newBrakingTarget) <= .01)
            {
                angularChange = -this.AngularVelocity;
            }

            return angularChange;
        }

        if (Input.GetKey(this.Player.RotLeft))
        {
            angularChange = this.RotationalThrust * Time.deltaTime;
        }

        if (Input.GetKey(this.Player.RotRight))
        {
            angularChange = -this.RotationalThrust * Time.deltaTime;
        }

        return angularChange;
    }

    private Vector2 CalculateThrustValue()
    {
        if (!this.Player.HasControl)
        {
            return Vector2.zero;
        }

        float thrustValueY = 0f;
        float thrustValueX = 0f;
        float thrustValue = this.Thrust * Time.deltaTime;

        if (Input.GetKey(this.Player.ThrustF))
        {
            thrustValueY += thrustValue;
        }

        if (Input.GetKey(this.Player.ThrustR))
        {
            thrustValueY -= thrustValue;
        }

        if (Input.GetKey(KeyCode.D))
        {
            thrustValueX += thrustValue;
        }

        if (Input.GetKey(KeyCode.A))
        {
            thrustValueX -= thrustValue;
        }

        return new Vector2(thrustValueX, thrustValueY);
    }

    private bool ThrusterKeyPressed()
    {
        return Input.GetKey(this.Player.ThrustF) || Input.GetKey(this.Player.ThrustR) || Input.GetKey(this.Player.BrakeThr);
    }

    private bool RotationKeyPressed()
    {
        return Input.GetKey(this.Player.RotLeft) || Input.GetKey(this.Player.RotRight) || Input.GetKey(this.Player.BrakeRot);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            this.Velocity = Vector2.zero;

            Vector3 direction = this.transform.position - other.transform.position;

            this.Velocity = new Vector2(
                direction.x * this.AsteroidRepulsionForce,
                direction.y * this.AsteroidRepulsionForce);
        }
    }

    private void HandleParticleEmitterStates(Vector2 velocityChange)
    {
        //Debug.Log($"Inc Velocity = {velocityChange.x}, {velocityChange.y}");
        //Debug.Log($"Transd Velocity = {z.x}, {z.y}");

        //velocityChange = this.transform.InverseTransformDirection(velocityChange);

        // **screaming internally so loud I'm going deaf**
        // Please don't hold this against me, I was desperate to make it work like I expected
        if (!velocityChange.x.WithinThreshold(this.floatingPointThreshold) && velocityChange.x < 0)
        {
            if (this.LeftEmitter.isPlaying)
            {
                this.LeftEmitter.Stop();
            }

            if (!this.RightEmitter.isPlaying)
            {
                this.RightEmitter.Play();
            }
        }

        if (!velocityChange.x.WithinThreshold(this.floatingPointThreshold) && velocityChange.x > 0)
        {
            if (this.RightEmitter.isPlaying)
            {
                this.RightEmitter.Stop();
            }

            if (!this.LeftEmitter.isPlaying)
            {
                this.LeftEmitter.Play();
            }
        }

        if (!velocityChange.y.WithinThreshold(this.floatingPointThreshold) && velocityChange.y > 0)
        {
            if (this.LeftRetroEmitter.isPlaying)
            {
                this.LeftRetroEmitter.Stop();
            }

            if (this.RightRetroEmitter.isPlaying)
            {
                this.RightRetroEmitter.Stop();
            }

            if (!this.MainEmitter.isPlaying)
            {
                this.MainEmitter.Play();
            }
        }

        if (!velocityChange.y.WithinThreshold(this.floatingPointThreshold) && velocityChange.y < 0)
        {
            if (this.MainEmitter.isPlaying)
            {
                this.MainEmitter.Stop();
            }

            if (!this.LeftRetroEmitter.isPlaying)
            {
                this.LeftRetroEmitter.Play();
            }

            if (!this.RightRetroEmitter.isPlaying)
            {
                this.RightRetroEmitter.Play();
            }
        }

        if (velocityChange.y.WithinThreshold(this.floatingPointThreshold))
        {
            if (this.MainEmitter.isPlaying)
            {
                this.MainEmitter.Stop();
            }

            if (this.LeftRetroEmitter.isPlaying)
            {
                this.LeftRetroEmitter.Stop();
            }

            if (this.RightRetroEmitter.isPlaying)
            {
                this.RightRetroEmitter.Stop();
            }
        }

        if (velocityChange.x.WithinThreshold(this.floatingPointThreshold))
        {
            if (this.LeftEmitter.isPlaying)
            {
                this.LeftEmitter.Stop();
            }

            if (this.RightEmitter.isPlaying)
            {
                this.RightEmitter.Stop();
            }
        }
    }
}

