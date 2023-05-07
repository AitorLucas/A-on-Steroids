using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI scoreText;

    private GameManager gameManager;

    private void Start() {
        gameManager = GameManager.Instance;

        gameManager.OnScoreChanged += GameManager_OnScoreChanged;
    }
    
    private void GameManager_OnScoreChanged(object sender, GameManager.OnScoreChangedArgs e) {
        scoreText.text = "SCORE: " + e.score.ToString();
    }
}
