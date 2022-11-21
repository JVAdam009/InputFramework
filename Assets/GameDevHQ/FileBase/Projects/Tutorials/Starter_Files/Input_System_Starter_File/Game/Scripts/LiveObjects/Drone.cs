using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;
using UnityEngine.EventSystems;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;

        private Vector2 movementDirection;
        private float ThrustDirection;

        public void SetMovementDirection(Vector2 dir)
        {
            movementDirection = dir;
        }
        public void SetThrustDirection(float dir)
        {
            ThrustDirection = dir;
        }

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                InputManager.Instance.ActivateDrone();
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {            
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);            
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();
            }
        }

        public void ExitDroneMode()
        {
            _inFlightMode = false;
            onExitFlightmode?.Invoke();
            ExitFlightMode();
            InputManager.Instance.ActivatePlayer();
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += movementDirection.x*( _speed / 3);
                transform.localRotation = Quaternion.Euler(tempRot);
        }

        private void CalculateMovementFixedUpdate()
        {
                _rigidbody.AddForce((ThrustDirection * transform.up) * _speed, ForceMode.Acceleration);
        }

        private void CalculateTilt()
        {
                transform.rotation = Quaternion.Euler(movementDirection.y * 30, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
