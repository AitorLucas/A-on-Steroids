using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Projectile projectileObject;

    private float linearMagnitude = 16f;
    private float angularMagnitude = 8f;
    private float maxLinearSpeed = 10f;
    private float maxAngularSpeed = 5f;
    [SerializeField] private float dragMagnitude = 2f;

    private float nextFireTime; 
    private float fireRate = 0.15f;
    private float burstDelay = 0.1f;
    private int burstCount = 3;
    private bool canFire = true;

    private Rigidbody playerRigidbody;

    public event EventHandler OnShootFired;

    private void Awake() {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleMovement();
        HandleFireShot();
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
            Projectile projectile = Instantiate<Projectile>(projectileObject, spawnPoint);
            Destroy(projectile, 3);

            float shotMagnitude = 30;
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * shotMagnitude, ForceMode.Impulse);

            OnShootFired?.Invoke(this, EventArgs.Empty);

            if (i < burstCount - 1) {
                yield return new WaitForSeconds(burstDelay);
            }
        }
        yield return new WaitForSeconds(fireRate - (burstCount - 1) * burstDelay);
        canFire = true;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.TryGetComponent(out Obstacle obstacle)) {
            // TODO: - DIMINUIR VIDA DO PLAYER


            // Debug.Log("Destroy Obstacle");
            // Destroy(obstacle.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out PowerUpVisual powerUp)) {
            Destroy(powerUp.gameObject);

            // TODO: - APLICAR EFEITO DO POWER UP
        }
    }
}
