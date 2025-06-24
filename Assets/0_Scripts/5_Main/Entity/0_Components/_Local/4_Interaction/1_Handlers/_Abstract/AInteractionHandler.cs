using UnityEngine;

namespace Badbarbos.Interaction.Handlers
{
    public abstract class AInteractionHandler : MonoBehaviour
    {
        public Interactable _interactable { get; set; }

        public virtual void Initialize(Interactable interactable)
        {
            _interactable = interactable;
        }

        public virtual void OnEnterRange(InteractableCandidate interactableCandidate) { }

        public virtual void OnExitRange(InteractableCandidate interactableCandidate) { }

        public virtual void OnTryInteract(InteractableCandidate interactableCandidate) { }
    }
}