using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.LiveObjects;
using Game.Scripts.Player;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InputManager();
            }

            return _instance;
        }
    }

    public  PlayerInputActions _input;

    private PlayerInputActions.PlayerActions _playerActions;

    public PlayerInputActions.IneractiveZonesActions _zonesActions;

    private PlayerInputActions.DroneActions _droneActions;

    private PlayerInputActions.ForkliftActions _forkliftActions;

    private PlayerInputActions.CrateActions _crateActions;

    [SerializeField] private Player _player;
    [SerializeField] private Drone _drone;
    [SerializeField] private Forklift _forklift;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitInput();
    }

    void CalculatePlayerMovement()
    {
        if(_playerActions.Movement != null)
        _player.CalcutateMovement(_playerActions.Movement.ReadValue<Vector2>());
    }

    private void Update()
    {
        CalculatePlayerMovement();
        CalculateDrone();
        CalculateForklift();
    }

    void CalculateDrone()
    {
        _drone.SetMovementDirection(_droneActions.WASD.ReadValue<Vector2>());
        _drone.SetThrustDirection(_droneActions.Thrust.ReadValue<float>());
    }

    void CalculateForklift()
    {
        _forklift.SetLift(_forkliftActions.Lift.ReadValue<float>());
        _forklift.SetMovement(_forkliftActions.WASD.ReadValue<Vector2>());
    }

    private void InitInput()
    {
        _input = new PlayerInputActions();
        _playerActions = _input.Player;
        _playerActions.Enable();
        _zonesActions = _input.IneractiveZones;
        _zonesActions.Enable();
        _zonesActions.Escape.performed += EscapeOnperformed;
        _droneActions = _input.Drone;
        _forkliftActions = _input.Forklift;
        _crateActions = _input.Crate;
    }

    private void EscapeOnperformed(InputAction.CallbackContext obj)
    {
        _drone.ExitDroneMode();
        _forklift.ExitDriveMode();
    }

    public void ActivatePlayer()
    {
        _droneActions.Disable();
        _forkliftActions.Disable();
        _playerActions.Enable();
        _zonesActions.Enable();
        _crateActions.Enable();
    }

    public void ActivateDrone()
    {
        _droneActions.Enable();
        _forkliftActions.Disable();
        _playerActions.Disable();
        _zonesActions.Enable();
        _crateActions.Disable();
    }

    public void ActivateForklift()
    {
        _droneActions.Disable();
        _forkliftActions.Enable();
        _playerActions.Disable();
        _zonesActions.Enable();
        _crateActions.Disable();
    }
}
