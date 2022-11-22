using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            
            if (_isReadyToBreak == false && _brakeOff.Count >0 && zone.GetZoneID() == 6)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
                Debug.Log("Ready");
            }
            else
            {
                if(_isReadyToBreak == true &&_brakeOff.Count == 0 && zone.GetZoneID() == 6)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        public void TapPunch()
        {
            if (_isReadyToBreak && _interactableZone.GetZoneID() == 6) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    Debug.Log("Tap");
                    BreakPart();
                    StartCoroutine(PunchDelay());
                }
                

                if (_brakeOff.Count <= 0)
                {
                    InteractableZone_onZoneInteractionComplete(_interactableZone);
                }
            }
        }

        public void HoldPunch()
        {
            if (_isReadyToBreak && _interactableZone.GetZoneID() == 6) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    Debug.Log("HoldPunch");
                    BreakPart();
                    BreakPart();
                    BreakPart();
                    BreakPart();
                    StartCoroutine(PunchDelay());
                }
                

                if (_brakeOff.Count <= 0)
                {
                    InteractableZone_onZoneInteractionComplete(_interactableZone);
                }
                
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);
            
        }



        public void BreakPart()
        {
            if(_brakeOff.Count <= 0) return;
            
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
