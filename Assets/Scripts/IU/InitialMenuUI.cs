using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitialMenuUI : MonoBehaviour {

    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        startButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}