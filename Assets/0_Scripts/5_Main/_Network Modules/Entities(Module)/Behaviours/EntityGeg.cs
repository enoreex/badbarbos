using UnityEngine.Events;
using UnityEngine;

namespace Badbarbos.Network.Modules.Entities
{
    public class EntityGeg : AEntityNetwork
    {
        [SerializeField] private UnityEvent _activated;

        public void Activate() => _activated.Invoke();
    }
}
