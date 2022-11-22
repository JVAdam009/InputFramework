using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Game.Scripts.LiveObjects
{
    public class Laptop : MonoBehaviour
    {
        [SerializeField]
        private Slider _progressBar;
        [SerializeField]
        private int _hackTime = 5;
        private bool _hacked = false;
        [SerializeField]
        private CinemachineVirtualCamera[] _cameras;
        private int _activeCamera = 0;
        [SerializeField]
        private InteractableZone _interactableZone;

        private PlayerInputActions.IneractiveZonesActions _actions;

        private bool interacting = false;
        private bool leaving = false;
        public static event Action onHackComplete;
        public static event Action onHackEnded;

        private void Start()
        {
            _actions = InputManager.Instance._input.IneractiveZones;
            _actions.Enable();
            _actions.Interact.performed += Interact;
            _actions.Interact.canceled += InteractStop;
            _actions.Escape.performed += Escape;
            _actions.Escape.canceled += EscapeStop;
        }

        private void EscapeStop(InputAction.CallbackContext obj)
        {
            if (_interactableZone.InZone())
            {
                leaving = false;
            }
        }

        private void InteractStop(InputAction.CallbackContext obj)
        {
            if (obj.interaction is HoldInteraction && _interactableZone.InZone())
            {
                InteractableZone_onHoldEnded(_interactableZone.GetZoneID());
            }
            else
            {
                interacting = false;
            }
        }

        private void Escape(InputAction.CallbackContext obj)
        {
            if (_interactableZone.InZone())
            {
                leaving = true;
            }
        }

        private void Interact(InputAction.CallbackContext obj)
        {
            if (obj.interaction is HoldInteraction && _interactableZone.InZone())
            {
                InteractableZone_onHoldStarted(_interactableZone.GetZoneID());
            }
            else if(obj.interaction is PressInteraction &&_interactableZone.InZone())
            {
                interacting = true;
                InteractableZone_onHoldStarted(_interactableZone.GetZoneID());
            }
        }


 

        private void Update()
        {
            if (_hacked == true)
            {
                if (interacting)
                {
                    var previous = _activeCamera;
                    _activeCamera++;


                    if (_activeCamera >= _cameras.Length)
                        _activeCamera = 0;


                    _cameras[_activeCamera].Priority = 11;
                    _cameras[previous].Priority = 9;
                    interacting = false;
                }

                if (leaving)
                {
                    _hacked = false;
                    onHackEnded?.Invoke();
                    ResetCameras();
                    leaving = false;
                }
            }
        }

        void ResetCameras()
        {
            foreach (var cam in _cameras)
            {
                cam.Priority = 9;
            }
        }

        private void InteractableZone_onHoldStarted(int zoneID)
        {
            if (zoneID == 3 && _hacked == false) //Hacking terminal
            {
                _progressBar.gameObject.SetActive(true);
                StartCoroutine(HackingRoutine());
                onHackComplete?.Invoke();
            }
        }

        private void InteractableZone_onHoldEnded(int zoneID)
        {
            if (zoneID == 3) //Hacking terminal
            {
                if (_hacked == true)
                    return;

                StopAllCoroutines();
                _progressBar.gameObject.SetActive(false);
                _progressBar.value = 0;
                onHackEnded?.Invoke();
            }
        }

        
        IEnumerator HackingRoutine()
        {
            while (_progressBar.value < 1)
            {
                _progressBar.value += Time.deltaTime / _hackTime;
                yield return new WaitForEndOfFrame();
            }

            //successfully hacked
            _hacked = true;
            _interactableZone.CompleteTask(3);

            //hide progress bar
            _progressBar.gameObject.SetActive(false);

            //enable Vcam1
            _cameras[0].Priority = 11;
        }
        

    }

}

