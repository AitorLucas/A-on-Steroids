using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject outerGlow;

    public event EventHandler OnPowerUpDestroy;

    private PowerUpType powerUpType;

    private void Update() {
        RotateVisual();
    }

    public void DefinePowerUpType(PowerUpType powerUpType) {
        this.powerUpType = powerUpType; 
    }
    
    public PowerUpType GetPowerUpType() {
        return powerUpType;
    }

    public void InstantKill() {
        OnPowerUpDestroy?.Invoke(this, EventArgs.Empty);
    }

    private void RotateVisual() {
        float rotationSpeed = 50f;
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }    
}
