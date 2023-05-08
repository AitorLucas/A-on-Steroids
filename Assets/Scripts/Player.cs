using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // - Singleton
    public static Player Instance { get; private set; }
    // - SerializeField
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightPUSpawnPoint;
    [SerializeField] private Transform leftPUSpawnPoint;
    [SerializeField] private Projectile projectileObject;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private ParticleSystem mainJetParticles;
    [SerializeField] private ParticleSystem leftJetParticles;
    [SerializeField] private ParticleSystem rightJetParticles;
    [SerializeField] private ParticleSystem frontLeftJetParticles;
    [SerializeField] private ParticleSystem frontRightJetParticles;
    // - Events
    public event EventHandler OnPlayerCrash;
    public event EventHandler OnShootFired;
    public event EventHandler<OnPlayerLifeChangedArgs> OnPlayerLifeChanged;
    public class OnPlayerLifeChangedArgs : EventArgs {
        public float currentLifeNormalized;
    }
    // - Movement
    private float linearMagnitude = 14f;
    private float angularMagnitude = 2f;
    private float maxLinearSpeed = 14f;
    private float maxAngularSpeed = 2f;
    private float dragMagnitude = 30f;
    // - Shot
    private float nextFireTime; 
    private float fireRate = 0.6f;
    private float burstDelay = 0.1f;
    private int burstCount = 3;
    private bool canFire = true;
    // - PowerUp
    private bool isPowerUpActive = false;
    private PowerUpType powerUpType;
    private float powerUpTimer;
    // - Life
    private float currentLife;
    private float maxLife = 500;

    private Rigidbody playerRigidbody;

    private void Awake() {
        Instance = this;

        currentLife = maxLife;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        inputManager.OnShotFireAction += InputManager_OnShotFireAction;

        mainJetParticles.Stop();
        leftJetParticles.Stop();
        rightJetParticles.Stop();
        frontLeftJetParticles.Stop();
        frontRightJetParticles.Stop();
    }

    private void Update() {
        HandleMovement();
        HandlePowerUp();
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.TryGetComponent(out Obstacle obstacle)) {
            if (isPowerUpActive) {
                if (powerUpType == PowerUpType.Invincibility) {
                    obstacle.InstantKill();
                    return;
                }
                if (powerUpType == PowerUpType.Shield) {
                    powerUpTimer = 0.15f;
                    return;
                }
            }

            float damage = 30f;
            AddDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out PowerUp powerUp)) {
            isPowerUpActive = true;
            powerUpType = powerUp.GetPowerUpType();
            powerUpTimer = 10;

            if (powerUpType == PowerUpType.Shield) {
                ShowShield();
            } else {
                HideShield();
            }

            if (powerUpType == PowerUpType.Life) {
                float life = 100f;
                AddLife(life);
            }

            Destroy(powerUp.gameObject);
        }
    }

    private void InputManager_OnShotFireAction(object sender, EventArgs e) {
        if (canFire && Time.time >= nextFireTime) {
            nextFireTime = Time.time + fireRate;
            StartCoroutine(FireBurst());
        }
    }

    private void HandleMovement() {
        Vector2 movement = inputManager.GetMovementVectorNormalized();

        if (playerRigidbody.velocity.magnitude < maxLinearSpeed) {
            float linearForce = linearMagnitude * movement.y;
            playerRigidbody.AddForce(this.transform.forward * linearForce);
        }        
        
        if (playerRigidbody.angularVelocity.magnitude < maxAngularSpeed) {
            float angularForce = angularMagnitude * movement.x;
            playerRigidbody.AddTorque(new Vector3(0, angularForce, 0));        
        }

        AddDrag();
        HandleJetParticles(movement);
    }

    private void AddDrag() {
        Vector3 direction = -playerRigidbody.velocity.normalized;
        float velocity = playerRigidbody.velocity.magnitude;

        playerRigidbody.AddForce(direction * velocity * dragMagnitude * Time.deltaTime);
    }

    private void HandleJetParticles(Vector2 movement) {
        if (movement.y > 0) {
            mainJetParticles.Play();
            frontLeftJetParticles.Stop();
            frontRightJetParticles.Stop();
            
            if (movement.x > 0) {
                leftJetParticles.Play();
                rightJetParticles.Stop();
            } else if (movement.x < 0) {
                rightJetParticles.Play();
                leftJetParticles.Stop();
            } else {
                leftJetParticles.Play();
                rightJetParticles.Play();
            }
        } else if (movement.y < 0) {
            mainJetParticles.Stop();
            leftJetParticles.Stop();
            rightJetParticles.Stop();
            
            if (movement.x > 0) {
                frontRightJetParticles.Play();
                frontLeftJetParticles.Stop();
            } else if (movement.x < 0) {
                frontLeftJetParticles.Play();
                frontRightJetParticles.Stop();
            } else {
                frontLeftJetParticles.Play();
                frontRightJetParticles.Play();
            }
        } else {    
            mainJetParticles.Stop();
            
            if (movement.x > 0) {
                leftJetParticles.Play();
                frontRightJetParticles.Play();
                rightJetParticles.Stop();
                frontLeftJetParticles.Stop();
            } else if (movement.x < 0) {
                rightJetParticles.Play();
                frontLeftJetParticles.Play();
                leftJetParticles.Stop();
                frontRightJetParticles.Stop();
            } else {
                rightJetParticles.Stop();
                frontLeftJetParticles.Stop();
                leftJetParticles.Stop();
                frontRightJetParticles.Stop();
            }
        }
    }

    IEnumerator FireBurst() {
        canFire = false;
        for (int i = 0; i < burstCount; i++) {
            ShotProjectile(leftSpawnPoint);
            ShotProjectile(rightSpawnPoint);

            if (isPowerUpActive && powerUpType == PowerUpType.DoubleFire) {
                ShotProjectile(leftPUSpawnPoint);
                ShotProjectile(rightPUSpawnPoint);
            }

            OnShootFired?.Invoke(this, EventArgs.Empty);

            if (i < burstCount - 1) {
                yield return new WaitForSeconds(burstDelay);
            }
        }
        yield return new WaitForSeconds(fireRate - (burstCount - 1) * burstDelay);
        canFire = true;
    }

    private void ShotProjectile(Transform spawnTransform) {
        Projectile projectile = Instantiate<Projectile>(projectileObject, spawnTransform);
        Destroy(projectile.gameObject, 1.5f);

        float shotMagnitude = 40;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * shotMagnitude, ForceMode.Impulse);
    }

    private void HandlePowerUp() {
        if (isPowerUpActive) {
            powerUpTimer -= Time.deltaTime;

            if (powerUpTimer <= 0) {
                isPowerUpActive = false;
            }
        } else {
            HideShield();
        }
    }

    private void AddDamage(float damage) {
        currentLife -= damage;

        Debug.Log("Damage");

        OnPlayerLifeChanged?.Invoke(this, new OnPlayerLifeChangedArgs {
            currentLifeNormalized = this.currentLife / this.maxLife
        });

        Debug.Log(currentLife);

        if (currentLife <= 0) {
            // TODO: - Explode ship          
            OnPlayerCrash?.Invoke(this, EventArgs.Empty);
        }
    }

    private void AddLife(float life) {
        currentLife += life;

        if (currentLife > maxLife) {
            currentLife = maxLife;
        }

        OnPlayerLifeChanged?.Invoke(this, new OnPlayerLifeChangedArgs { 
            currentLifeNormalized = this.currentLife / this.maxLife
        });
    }

    private void HideShield() {
        shieldObject.gameObject.SetActive(false);
    }

    private void ShowShield() {
        shieldObject.gameObject.SetActive(true);
    }
}
