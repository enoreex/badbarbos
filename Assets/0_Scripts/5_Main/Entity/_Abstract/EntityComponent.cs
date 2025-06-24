using UnityEngine;

namespace Badbarbos.Player.Components
{
    public class EntityComponent : MonoBehaviour
    {
        protected Entity _player;

        public void Link(Entity player)
        {
            _player = player;
        }

        public void Enable(Entity player) { Enable(); }

        public void Disable(Entity player) { Disable(); }

        protected virtual void Enable() { }

        protected virtual void Disable() { }
    }
}
