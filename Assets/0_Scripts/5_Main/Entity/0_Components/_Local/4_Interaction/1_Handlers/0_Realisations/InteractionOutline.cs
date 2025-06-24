using UnityEngine;

using TMPro;

namespace Badbarbos.Interaction.Handlers
{
    public class InteractionOutline : AInteractionHandler
    {
        [SerializeField] private Outline _outline;

        public override void OnEnterRange(InteractableCandidate interactableCandidate) => _outline.enabled = true;

        public override void OnExitRange(InteractableCandidate interactableCandidate) => _outline.enabled = false;

        private void OnDisable() => _outline.enabled = false;
    }
}