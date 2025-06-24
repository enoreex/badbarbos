using UnityEngine;

using TMPro;

namespace Badbarbos.Interaction.Handlers
{
    public class InteractionHint : AInteractionHandler
    {
        [SerializeField] private GameObject _hintPrefab;

        [Header("Text Settings")]
        [SerializeField] private string _hintText = "Press E to interact";
        [SerializeField] private float _fontSize = 2f;

        [SerializeField] private Vector3 _positionOffset = new Vector3(0, 1f, 0);

        private GameObject _currentHintInstance;

        private TextMeshPro _hintTextComponent;

        private InteractableCandidate _currentInteractableCandidate;

        public override void OnEnterRange(InteractableCandidate interactableCandidate)
        {
            if (_hintPrefab == null) return;

            if (_currentHintInstance == null)
            {
                _currentHintInstance = Instantiate(_hintPrefab,
                    transform.position + _positionOffset,
                    Quaternion.identity);

                _hintTextComponent = _currentHintInstance.GetComponent<TextMeshPro>();

                _hintTextComponent.text = _hintText;
                _hintTextComponent.fontSize = _fontSize;
            }

            _currentHintInstance.SetActive(true);
            _currentInteractableCandidate = interactableCandidate;
        }

        public override void OnExitRange(InteractableCandidate interactableCandidate)
        {
            if (_currentHintInstance != null)
            {
                Destroy(_currentHintInstance);
            }
            _currentInteractableCandidate = null;
        }

        public void Update()
        {
            if (_currentInteractableCandidate == null || _currentHintInstance == null) return;

            _currentHintInstance.transform.LookAt(_currentInteractableCandidate.CurrentCamera.transform);

            _currentHintInstance.transform.position = transform.position + _positionOffset;
        }

        private void OnDisable()
        {
            if (_currentHintInstance != null)
            {
                Destroy(_currentHintInstance);
            }
        }
    }
}