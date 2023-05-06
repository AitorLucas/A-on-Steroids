using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject outerGlow;

    private void Update() {
        RotateVisual();
    }

    private void RotateVisual() {
        float rotationSpeed = 50f;
        visual.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
