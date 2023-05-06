using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpVisual : MonoBehaviour {

    [SerializeField] private PowerUp powerUp;

    private void OnDestroy() {
        Destroy(powerUp.gameObject);
    }
}
