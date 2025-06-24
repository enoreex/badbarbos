using System.Collections.Generic;

using UnityEngine;

namespace Badbarbos.Player
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private List<Components.EntityComponent> _components = new List<Components.EntityComponent>();

        public void Awake() => _components.ForEach(x => x.Link(GetComponent<Entity>()));

        public void EnableModule(Components.EntityComponent behaviour)
        {
            if (_components.Contains(behaviour) is false) return;

            behaviour.Enable(this);
            behaviour.gameObject.SetActive(true);
        }

        public void DisableModule(Components.EntityComponent behaviour)
        {
            if (_components.Contains(behaviour) is false) return;

            behaviour.Disable(this);
            behaviour.gameObject.SetActive(false);
        }
    }
}