using Badbarbos.Network.Modules.Members;

using ICENet.Traffic;

using UnityEngine;
using UnityEngine.Events;

namespace Badbarbos.Network.Modules.Entities
{
    public class EntityProp : AEntityNetwork
    {
        public Member Owner { get; set; } = null!;

        #region

        [SerializeField] private UnityEvent _youEntered;

        [SerializeField] private UnityEvent _anotherEntered;

        [SerializeField] private UnityEvent _exit;

        public void YouEnter() => _youEntered.Invoke();

        public void YouExit() => _exit.Invoke();

        public void AnotherEnter() => _anotherEntered.Invoke();

        public void AnotherExit() => _exit.Invoke();

        #endregion

        #region

        [SerializeField] private UnityEvent<Buffer> _getData;

        [SerializeField] private UnityEvent<Buffer> _setData;

        public Buffer GetData()
        {
            Buffer buffer = new Buffer(0);

            _getData.Invoke(buffer);

            return buffer;
        }

        public void SetData(Buffer buffer)
        {
            _setData.Invoke(buffer);
        }

        #endregion
    }
}
