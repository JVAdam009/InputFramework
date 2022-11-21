using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.LiveObjects;
using Game.Scripts.Player;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

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

    [SerializeField] private Player _player;


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
    }

    private void InitInput()
    {
        _input = new PlayerInputActions();
        _playerActions = _input.Player;
        _playerActions.Enable();
        _zonesActions = _input.IneractiveZones;
        _zonesActions.Enable();
    }
}
