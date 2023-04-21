using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvasUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI velocityText;

    private Player player;

    private void Start() {
        player = Player.Instance;
    }

    private void Update() {
        velocityText.text = "Velocity: ";
    }
}
