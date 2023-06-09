using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out Obstacle obstacle)) {
            Destroy(gameObject);
        }
    }
}
