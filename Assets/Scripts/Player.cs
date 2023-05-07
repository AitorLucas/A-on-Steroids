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
    // - Events
    public event EventHandler OnPlayerCrash;
    public event EventHandler OnShootFired;
    // - Movement
    private float linearMagnitude = 20f;
    private float angularMagnitude = 3f;
    private float maxLinearSpeed = 10f;
    private float maxAngularSpeed = 3f;
    [SerializeField] private float dragMagnitude = 2f;
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

    private Rigidbody playerRigidbody;

    private void Awake() {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleMovement();
        HandleFireShot();
        HandlePowerUp();
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
    }

    private void AddDrag() {
        Vector3 direction = -playerRigidbody.velocity.normalized;
        float velocity = playerRigidbody.velocity.magnitude;

        playerRigidbody.AddForce(direction * velocity * dragMagnitude * Time.deltaTime);
    }

    private void HandleFireShot() {
        if (Input.GetKeyDown(KeyCode.Space) && canFire && Time.time >= nextFireTime) {
            nextFireTime = Time.time + fireRate;
            StartCoroutine(FireBurst());
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
        Destroy(projectile, 3);

        float shotMagnitude = 30;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * shotMagnitude, ForceMode.Impulse);
    }

    private void HandlePowerUp() {
        if (isPowerUpActive) {
            powerUpTimer -= Time.deltaTime;

            if (powerUpTimer <= 0) {
                isPowerUpActive = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.TryGetComponent(out Obstacle obstacle)) {
            OnPlayerCrash?.Invoke(this, EventArgs.Empty);

            if (isPowerUpActive) {
                if (powerUpType == PowerUpType.Invincibility) {
                    obstacle.InstantKill();
                }
                if (powerUpType == PowerUpType.Shield) {
                    powerUpTimer = 0.3f;
                }
            }

            // Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out PowerUp powerUp)) {
            isPowerUpActive = true;
            powerUpType = powerUp.GetPowerUpType();
            powerUpTimer = 10;

            Destroy(powerUp.gameObject);
        }
    }
}
