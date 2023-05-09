using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button menuButton;

    private Player player;
    private GameManager gameManager;

    private void Start() {
        gameObject.SetActive(false);

        player = Player.Instance;
        gameManager = GameManager.Instance;

        player.OnPlayerCrash += Player_OnPlayerCrash;
        gameManager.OnScoreChanged += GameManager_OnScoreChanged;

        menuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.InitialScene);
        });
    }
    
    private void GameManager_OnScoreChanged(object sender, GameManager.OnScoreChangedArgs e) {
        scoreText.text = "SCORE: " + e.score.ToString();
    }

    private void Player_OnPlayerCrash(object sender, EventArgs e) {
        gameObject.SetActive(true);
    }
}
