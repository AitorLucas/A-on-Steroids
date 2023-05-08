using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    [SerializeField] private ObstacleVisual obstacleOuterVisual;

    public event EventHandler<OnObstacleDestroyArgs> OnObstacleDestroy;
    public class OnObstacleDestroyArgs : EventArgs {
        public float life;
    }

    private float life;
    private float currentLife;

    private float forceMagnitude;
    private float torqueMagnitude;

    private void Awake() {
        forceMagnitude = UnityEngine.Random.Range(-4.5f, 4.5f);
        torqueMagnitude = UnityEngine.Random.Range(-1.5f, 1.5f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out Projectile projectile)) {
            float damage = 20f;
            AddDamage(damage);
        }
    }

    public void InstantKill() {
        AddDamage(life);
    }

    private void AddDamage(float damage) {
        currentLife -= damage;
        if (currentLife <= 0) {
            OnObstacleDestroy?.Invoke(this, new OnObstacleDestroyArgs { life = this.life });
        } else {
            StartCoroutine(InitiateOuterEffect());
        }
    }

    private IEnumerator InitiateOuterEffect() {
        obstacleOuterVisual.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        obstacleOuterVisual.gameObject.SetActive(false);
    }

    public void DefineLife(float life) {
        this.life = life;
        this.currentLife = life;
    }

    public void ApplyForce() {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
            rigidbody.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)) * forceMagnitude, ForceMode.Impulse);
        }
    }

    public void ApplyTorque() {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
            rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * torqueMagnitude, ForceMode.Impulse);
        }
    }
}