using Badbarbos.Main;
using Badbarbos.Network.Modules.Entities;

namespace Badbarbos.Interaction.Handlers
{
    public class InteractionGegTryActivate : AInteractionHandler
    {
        public override void OnTryInteract(InteractableCandidate interactableCandidate) => FindFirstObjectByType<MainClient>().TryActivateGeg(_interactable.GetComponentInParent<EntityGeg>().Id);
    }
}