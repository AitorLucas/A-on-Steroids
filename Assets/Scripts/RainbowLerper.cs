using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowLerper : MonoBehaviour {

    [SerializeField] private Material material;

    public Color[] colors = {
        new Color(190, 0, 0, 0.2f),
        new Color(255, 165, 0, 0.2f),
        new Color(190, 190, 0, 0.2f),
        new Color(0, 190, 0, 0.2f),
        new Color(0, 0, 190, 0.2f),
        new Color(120, 0, 190, 0.2f),
        new Color(100, 20, 200, 0.2f),
    };

    private void Start() {
        StartCoroutine(CycleColor());
    }

    private IEnumerator CycleColor() {
        int index = 0;
        while (true) {
            for (float interpolant = 0f; interpolant < 1f; interpolant += 0.05f) {
                Color newColor = Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], interpolant);
                material.SetColor("_BaseColor", newColor);
                material.SetColor("_Color", newColor);
                yield return null;
            }
            index += 1;
        }
    }
}