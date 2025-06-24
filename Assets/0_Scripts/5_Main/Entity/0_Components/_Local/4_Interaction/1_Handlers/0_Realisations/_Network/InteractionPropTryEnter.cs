using Badbarbos.Main;
using Badbarbos.Network.Modules.Entities;

namespace Badbarbos.Interaction.Handlers
{
    public class InteractionPropTryEnter : AInteractionHandler
    {
        public override void OnTryInteract(InteractableCandidate interactableCandidate) => FindFirstObjectByType<MainClient>().TryEnterProp(_interactable.GetComponentInParent<EntityProp>().Id);
    }
}