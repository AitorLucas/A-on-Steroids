using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager Instance { get; private set; }

    private GameInputActions gameInputActions;

    public event EventHandler OnShotFireAction;

    private void Awake() {
        Instance = this;

        gameInputActions = new GameInputActions();
        gameInputActions.Player.Enable();

        gameInputActions.Player.ShotFire.performed += ShotFire_Performed;
    }

    private void OnDestroy() {
        gameInputActions.Player.ShotFire.performed -= ShotFire_Performed;

        gameInputActions.Dispose();
    }

    private void ShotFire_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnShotFireAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = gameInputActions.Player.Movement.ReadValue<Vector2>();
        return inputVector.normalized;
    }
}
