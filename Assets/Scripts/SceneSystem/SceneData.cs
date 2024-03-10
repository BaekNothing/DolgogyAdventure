using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ObjectSystem;
using Cysharp.Threading.Tasks;

namespace SceneSystem
{
    public class SceneData : MonoBehaviour
    {
        [SerializeField] List<ASceneObjectBase> _sceneObjects;

        void Start()
        {
            Initialize();
        }

        async void Initialize()
        {
            await UniTask.WaitUntil(() => CoreSystem.SystemRoot.IsInitialized);
            _sceneObjects = Utility.ChildFinder.GetAll<ASceneObjectBase>(transform);
            _sceneObjects.ForEach((sceneObject) => sceneObject.Initialize());
        }


    }
}
