using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image lifeBarImage;

    private GameManager gameManager;
    private Player player;

    private void Start() {
        gameManager = GameManager.Instance;
        player = Player.Instance;

        gameManager.OnScoreChanged += GameManager_OnScoreChanged;
        player.OnPlayerLifeChanged += Player_OnPlayerLifeChanged;
    }
    
    private void GameManager_OnScoreChanged(object sender, GameManager.OnScoreChangedArgs e) {
        scoreText.text = "SCORE: " + e.score.ToString();
    }

    private void Player_OnPlayerLifeChanged(object sender, Player.OnPlayerLifeChangedArgs e) {
        lifeBarImage.fillAmount = e.currentLifeNormalized;
    }
}
