using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

#pragma warning disable IDE0044 // ignore warning about private readonly field

namespace CoreSystem
{
    public class SystemRoot : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static IDataContainor DataContainor { get => _dataContainor; }
        public static IUIContainor UIContainor { get => _uiContainor; }
        public static IUIViewStackContainor UIViewStackContainor { get => _uiViewStackContainor; }

        static DataContainor _dataContainor;
        static UIContainor _uiContainor;
        static UIViewStackContainor _uiViewStackContainor;

        public static bool IsInitialized { get; private set; } = false;

#if UNITY_EDITOR
        public ScriptableObject[] Containors;
#endif

        private void Start()
        {
            InitAllContainor();
        }

        async void InitAllContainor()
        {
            await UniTask.WaitUntil(() => Utility.ChildDestoryer.IsAllDestoryerInited);
            Utility.Logger.Log($"CoreSystem.LoadAllContainor Start");

            LoadAllContainor();

            _dataContainor.Initialized();
            _uiContainor.Initialized();
            _uiViewStackContainor.Initialized();

            IsInitialized = true;
        }

        void LoadAllContainor()
        {
            var containors = Resources.LoadAll<ScriptableObject>("CoreSystem") ??
                throw new System.NullReferenceException($"CoreSystem.LoadAllContainor: containors is null");

            foreach (var containor in containors)
            {
                if (containor is DataContainor dataContainor)
                    _dataContainor = dataContainor;
                else if (containor is UIContainor uiContainor)
                    _uiContainor = uiContainor;
                else if (containor is UIViewStackContainor uiViewStackContainor)
                    _uiViewStackContainor = uiViewStackContainor;
                else
                    throw new System.NotImplementedException
                        ($"CoreSystem.LoadAllContainor: {containor.name} is not implemented");
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            Containors = new ScriptableObject[]
            {
                _dataContainor,
                _uiContainor,
                _uiViewStackContainor
            };
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
