using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField] private float linearMagnitude = 16f;
    [SerializeField] private float angularMagnitude = 8f;
    [SerializeField] private float maxLinearSpeed = 10f;
    [SerializeField] private float maxAngularSpeed = 5f;
    [SerializeField] private float fireDelay = 0.1f;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Projectile projectileObject;

    private float fireTimer;
    private Rigidbody playerRigidbody;

    public event EventHandler OnShootFired;

    private void Awake() {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleMovement();
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

        if (Input.GetKey(KeyCode.Space)) {
            if (Time.time >= fireTimer + fireDelay) {
                Projectile projectile = Instantiate<Projectile>(projectileObject, spawnPoint);
                // projectile.GetComponent<Rigidbody>().velocity = playerRigidbody.velocity;    

                float shotMagnitude = 30;
                projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * shotMagnitude, ForceMode.Impulse);
                
                fireTimer = Time.time;

                OnShootFired?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.TryGetComponent(out Obstacle obstacle)) {
            // DIMINUIR VIDA DO PLAYER


            // Debug.Log("Destroy Obstacle");
            // Destroy(obstacle.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out PowerUpVisual powerUp)) {
            Destroy(powerUp.gameObject);

            // APLICAR EFEITO DO POWER UP
        }
    }
}
