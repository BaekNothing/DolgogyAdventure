using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace CoreSystem
{
    public class CoreSystem : MonoBehaviour
    {
        public static IDataContainor DataContainor { get => _dataContainor; }
        public static IUIContainor UIContainor { get => _uiContainor; }

        static DataContainor _dataContainor;
        static UIContainor _uiContainor;

        private void Awake()
        {
            LoadAllContainor();

            _dataContainor.Initialized();
            _uiContainor.Initialized();
        }

        void LoadAllContainor()
        {
            UniTask.WaitUntil(() => Utility.ChildDestoryer.IsAllDestoryerInited);

            Utility.Logger.Log($"CoreSystem.LoadAllContainor Start");

            var containors = Resources.LoadAll<ScriptableObject>("CoreSystem") ??
                throw new System.NullReferenceException($"CoreSystem.LoadAllContainor: containors is null");

            foreach (var containor in containors)
            {
                if (containor is DataContainor dataContainor)
                    _dataContainor = dataContainor;
                else if (containor is UIContainor uiContainor)
                    _uiContainor = uiContainor;
            }
        }
    }
}
