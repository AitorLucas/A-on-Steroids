using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    float life;

    public static event EventHandler OnObstacleDestroy;

    private void OnDestroy() {
        OnObstacleDestroy?.Invoke(this, EventArgs.Empty);
    }

    public void InflictDamage(float damage) {
        life -= damage;

        if (life <= 0) {
            Destroy(this);
        }
    }
}