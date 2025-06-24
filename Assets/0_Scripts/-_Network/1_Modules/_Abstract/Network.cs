using ICENet;

using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;

namespace Badbarbos.Network
{

    public class Network : MonoBehaviour
    {
        [SerializeField] private IceClient _socket;

        private Dictionary<Type, ANetworkModuleClient> _modules = new();

        private void Awake() => SceneManager.activeSceneChanged += Initialize;

        private void OnDestroy() => SceneManager.activeSceneChanged -= Initialize;

        private void Initialize(Scene oldScene, Scene newScene)
        {
            DontDestroyOnLoad(gameObject);

            _modules ??= new Dictionary<Type, ANetworkModuleClient>();
            _modules.Clear();

            var all = FindObjectsByType<ANetworkModuleClient>(FindObjectsSortMode.None);

            _modules = all.ToDictionary(m => m.GetType());

            foreach (var module in _modules.Values) module.Setup(_socket);

            foreach (var module in _modules.Values)
            {
                var attrs = module.GetType().GetCustomAttributes<RequireModuleAttribute>(inherit: true);

                foreach (var attr in attrs)
                {
                    if (!_modules.TryGetValue(attr.ModuleType, out var dependency)) continue;

                    var t = module.GetType();

                    var prop = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(p => p.PropertyType == attr.ModuleType && p.CanWrite);
                    if (prop != null)
                    {
                        prop.SetValue(module, dependency);
                        continue;
                    }

                    var field = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(f => f.FieldType == attr.ModuleType);
                    if (field != null)
                    {
                        field.SetValue(module, dependency);
                        continue;
                    }
                }
            }

            FindObjectsByType<AClient>(FindObjectsSortMode.None).ToList().ForEach(x => x.Initialize(GetComponent<Network>()));
        }

        public T GetModuleClient<T>() where T : ANetworkModuleClient => _modules.TryGetValue(typeof(T), out var m) ? (T)m : null;

        public void Close() { Destroy(gameObject); SceneManager.LoadScene(0); }
    }
}