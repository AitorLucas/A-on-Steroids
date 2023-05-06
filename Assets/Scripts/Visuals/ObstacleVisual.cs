using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleVisual : MonoBehaviour {
    
    [SerializeField] private Obstacle obstacle;

    private void OnDestroy() {
        Destroy(obstacle.gameObject);
    }
}
