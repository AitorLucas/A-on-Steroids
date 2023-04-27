using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField] private float linearMagnitude = 8f;
    [SerializeField] private float angularMagnitude = 4f;
    [SerializeField] private InputManager inputManager;
    
    private Rigidbody playerRigidbody;
    [SerializeField] private float maxLinearSpeed = 8f;
    [SerializeField] private float maxAngularSpeed = 1f;

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

        // Debug.Log("Linear: " + playerRigidbody.velocity.magnitude + ", Angular: " + playerRigidbody.angularVelocity.magnitude);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.TryGetComponent(out Obstacle obstacle)) {
            Debug.Log("Destroy Obstacle");
            Destroy(obstacle.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out PowerUp powerUp)) {
            Debug.Log("Destroy PowerUp");
            Destroy(powerUp.gameObject);
        }
    }
}
