using Badbarbos.Network;
using Badbarbos.Network.Modules.Entities;
using Badbarbos.Network.Modules.Members;

using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Badbarbos.Main
{
    public class MainClient : AClient
    {
        private MembersModuleClient _membersModule;

        private EntitiesModuleClient _entitiesModule;

        public List<EntityProp> Props;

        public List<EntityGeg> Gegs;

        private List<EntityProp> _currentAnotherProps = new List<EntityProp>();

        private EntityProp _currentProp = null;

        public override void Initialize()
        {
            _membersModule = GetModule<MembersModuleClient>();
            _entitiesModule = GetModule<EntitiesModuleClient>();

            _entitiesModule.Enter();

            _entitiesModule.YouEnteredProp.AddListener(YouEnteredProp);
            _entitiesModule.AnotherEnteredProp.AddListener(AnotherEnteredProp);
            _entitiesModule.PropDataReceived.AddListener(PropDataReceived);
            _entitiesModule.GegActivated.AddListener(GegActivated);

            _membersModule.Update.AddListener((e) =>
            {
                Destroy(FindFirstObjectByType<Network.Network>().gameObject);

                Destroy(gameObject);

                SceneManager.LoadScene(0);
            });

            var myId = _membersModule.GetMyId();

            TryEnterProp(myId);
        }

        #region

        private void YouEnteredProp(int entityId)
        {
            var propToEnter = Props.FirstOrDefault(x => x.Id == entityId);

            if (propToEnter == null) return;

            if (_currentProp != null) _currentProp.YouExit();

            propToEnter.Owner = _membersModule.GetMember(_membersModule.GetMyId());
            propToEnter.YouEnter();

            if (_membersModule.GetMyId() != 1)
            {
                if (_currentProp) Destroy(_currentProp.GetComponent<InteractableCandidate>());

                propToEnter.AddComponent<InteractableCandidate>();

                propToEnter.GetComponent<InteractableCandidate>().CurrentCamera = propToEnter.GetComponentInChildren<Camera>();
            }    

            _currentProp = propToEnter;
        }

        private void AnotherEnteredProp(int memberId, int entityId)
        {
            var oldProp = _currentAnotherProps.FirstOrDefault(x => x.Owner.Id == memberId);

            if (oldProp != null) oldProp.AnotherExit();

            var propToEnter = Props.FirstOrDefault(x => x.Id == entityId);

            if (propToEnter == null) return;

            propToEnter.Owner = _membersModule.GetMember(memberId);
            propToEnter.AnotherEnter();

            if (oldProp != null) _currentAnotherProps.Remove(oldProp);

            _currentAnotherProps.Add(propToEnter);
        }

        private void GegActivated(int memberId, int entityId)
        {
            var geg = Gegs.FirstOrDefault(x => x.Id == entityId);

            if (geg == null) return;

            geg.Activate();
        }

        public void TryEnterProp(int entityId) => _entitiesModule.SendPropEnterRequest(entityId);

        public void TryActivateGeg(int entityId) => _entitiesModule.SendGegActivationRequest(entityId);

        #endregion

        #region Update

        private float _sendTimer = 0f;

        [SerializeField] private float _sendMyDataInterval = 0.2f; 

        private void Update()
        {
            _sendTimer += Time.deltaTime;

            if (_sendTimer < _sendMyDataInterval) return;
            _sendTimer -= _sendMyDataInterval;

            if (_currentProp == null) return;

            var data = _currentProp.GetData().ToArray();
            _entitiesModule.SendClientToServerPropDataPacket(data);
        }

        private void PropDataReceived(int memberId, int entityId, byte[] buffer)
        {
            var prop = Props.FirstOrDefault(x => x.Id == entityId);

            if (prop == null) return;

            var data = new ICENet.Traffic.Buffer(0);

            data.LoadBytes(buffer);

            prop.SetData(data);
        }

        #endregion
    }
}
