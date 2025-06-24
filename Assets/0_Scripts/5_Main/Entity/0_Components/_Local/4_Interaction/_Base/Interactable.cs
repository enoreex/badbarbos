using Badbarbos.Interaction.Handlers;

using Badbarbos.Player.Components;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Badbarbos.Interaction
{
    public class Interactable : EntityComponent
    {
        [Header("Hint")]
        [SerializeField] private KeyCode _interactKey = KeyCode.E;

        [Header("Cooldown")]
        [SerializeField] private float _cooldown = 3f;

        [Header("Components")]
        [SerializeField] private List<AInteractionHandler> _interactableHandlers = new List<AInteractionHandler>();

        private InteractableCandidate _currentInteractableCandidate;

        private bool _isOnCooldown;
        
        private void Awake()
        {
            foreach (var component in _interactableHandlers)
            {
                component.gameObject.SetActive(true);
                component.Initialize(this);
            }
        }

        private void Update()
        {
            if (_currentInteractableCandidate == null) return;

            if (Input.GetKeyDown(_interactKey))
            {
                TryInteract();
            }
        }

        public void TryInteract(InteractableCandidate candidate = null)
        {
            candidate ??= _currentInteractableCandidate;

            if (_isOnCooldown) return;

            foreach (var component in _interactableHandlers) component.OnTryInteract(candidate);

            StartCoroutine(CooldownRoutine());
        }

        public void OnTriggerEnter(Collider other)
        {
            var candidate = other.GetComponentInParent<InteractableCandidate>();
            if (candidate != null)
            {
                _interactableHandlers.ForEach(h => h.OnEnterRange(candidate));
                _currentInteractableCandidate = candidate;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            var exited = other.GetComponentInParent<InteractableCandidate>();
            if (exited != null && _currentInteractableCandidate == exited)
            {
                _interactableHandlers.ForEach(h => h.OnExitRange(_currentInteractableCandidate));
                _currentInteractableCandidate = null;
            }
        }

        private IEnumerator CooldownRoutine()
        {
            _isOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            _isOnCooldown = false;
        }

        private void OnEnable() => _isOnCooldown = false;

        private void OnDisable() => _isOnCooldown = false;
    }
}