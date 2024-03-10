using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

#pragma warning disable IDE0044 // ignore warning about private readonly field

namespace CoreSystem
{
    public class SystemRoot : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static IDataContainor Data { get => _dataContainor; }
        public static IUIContainor UI { get => _uiContainor; }
        public static IInputContainor Input { get => _inputContainor; }

        static DataContainor _dataContainor;
        static UIContainor _uiContainor;
        static InputContainor _inputContainor;

        public static bool IsInitialized { get; private set; } = false;

#if UNITY_EDITOR
        public ScriptableObject[] Containors;
#endif

        private void Start()
        {
            InitAllContainor();
        }

        private void Update()
        {
            if (!IsInitialized) return;

            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                if (UnityEngine.Input.GetKey(keyCode))
                    _inputContainor.Invoke(keyCode);
        }

        async void InitAllContainor()
        {
            await UniTask.WaitUntil(() => Utility.ChildDestoryer.IsAllDestoryerInited);
            Utility.Logger.Log($"CoreSystem.LoadAllContainor Start");

            LoadAllContainor();

            _dataContainor.Initialized();
            _uiContainor.Initialized();
            _inputContainor.Initialized();

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
                else if (containor is UIContainor uIContainor)
                    _uiContainor = uIContainor;
                else if (containor is InputContainor inputContainor)
                    _inputContainor = inputContainor;
                else
                    continue;
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            Containors = new ScriptableObject[]
            {
                _dataContainor,
                _uiContainor
            };
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
