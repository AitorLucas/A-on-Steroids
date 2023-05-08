using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType {
    Shield,
    Invincibility,
    DoubleFire,
    Life,
}

[CreateAssetMenu()]
public class PowerUpSO : ScriptableObject {

    public PowerUp powerUp;
    public string powerUpName;
    public PowerUpType powerUpType;

}
