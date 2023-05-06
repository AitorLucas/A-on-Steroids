using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] private ObstacleSO[] obstacleSOArray;

    private List<Obstacle> obstacleSpawnedList = new List<Obstacle>();

    // change name
    private float minRenderDistance = 20f;
    private float maxRenderDistance = 60f;
    private int obstaclesMax = 100;

    private void Start() {
        Vector3 playerPosition = Player.Instance.transform.position;
        SpawnObstacles(); 

        for (int index = 0; index < GetObstacleSpawned(); index ++) {
            Obstacle obstacle = obstacleSpawnedList[index];
            
            // obstacle.ApplyTranslation();
            // obstacle.ApplyRotation();
        }
    }

    private void Update() {
        RemoveFarObstacles();
    }

    private void SpawnObstacles() {
        Vector3 playerPosition = Player.Instance.transform.position;

        
        while(GetObstacleSpawned() < obstaclesMax) {
            // - Calc a Range around player subtracting a span near it.
            float radius = UnityEngine.Random.Range(minRenderDistance, maxRenderDistance);
            float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);

            Vector3 spawnPosition = playerPosition + new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);

            // - Spawn New Obstacle
            ObstacleSO newObstacleSO = obstacleSOArray[UnityEngine.Random.Range(0, obstacleSOArray.Length)];
            Obstacle newObstacle = Instantiate<Obstacle>(newObstacleSO.obstacle, spawnPosition, Quaternion.identity);
            newObstacle.OnObstacleDestroy += Obstacle_OnObstacleDestroy;
        
            // - Modify scale and life
            float scale = newObstacleSO.baseScale * UnityEngine.Random.Range(0.5f, 2f);
            newObstacle.transform.localScale = new Vector3(scale, scale, scale);
            newObstacle.DefineInitialLife(newObstacleSO.baseLife * scale);

            // - Add movement to obstacle
            newObstacle.ApplyForce();
            newObstacle.ApplyTorque();

            // - Add to array to validate distance afterwards
            obstacleSpawnedList.Add(newObstacle);        
        }
    }

    private void Obstacle_OnObstacleDestroy(object sender, EventArgs e) {
        Obstacle obstacle = sender as Obstacle;
        obstacleSpawnedList.Remove(obstacle);

        obstacle.DestroySelf();

        SpawnObstacles();
    }

    private void RemoveFarObstacles() {
        Vector3 playerPosition = Player.Instance.transform.position;

        for (int index = 0; index < GetObstacleSpawned(); index ++) {
            Obstacle obstacle = obstacleSpawnedList[index];
            float distance = Vector3.Distance(obstacle.transform.position, playerPosition);
            if (distance > maxRenderDistance) {    
                obstacleSpawnedList.RemoveAt(index);
                index -= 1;

                obstacle.DestroySelf();
            }
        }

        SpawnObstacles();      
    }

    private int GetObstacleSpawned() {
        return obstacleSpawnedList.Count;
    }
}
