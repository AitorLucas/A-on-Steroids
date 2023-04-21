using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private GameInputActions gameInputActions;

    private void Awake() {
        gameInputActions = new GameInputActions();
        gameInputActions.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector =  gameInputActions.Player.Movement.ReadValue<Vector2>();

        Debug.Log(inputVector);

        return inputVector.normalized;
    }
}
