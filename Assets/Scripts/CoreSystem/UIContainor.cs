using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CoreSystem
{
    public interface IUIContainor
    {
        GameObject GetUIPrefab(string name);
        GameObject GetUICanvas();
    }

    [CreateAssetMenu(fileName = "UIContainor", menuName = "ScriptableObjects/UIContainor", order = 1)]
    public class UIContainor : ScriptableObject, ISerializationCallbackReceiver, IUIContainor
    {
        public static bool IsInitialized { get; private set; } = false;

        Dictionary<string, GameObject> _uiPrefabs = new();
        public string[] UIName;
        const string _uiPath = "UI";
        const string _uiCanvasName = "UICanvas";

        public void Initialized()
        {
            if (IsInitialized)
                return;

            var DirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/{_uiPath}") ??
                throw new NullReferenceException($"UIContainor.Initialized: {_uiPath} not found");

            _uiPrefabs.Clear();
            var dirPaths = DirInfo.GetDirectories().Select(dir => dir.Name).ToArray();
            foreach (var dirPath in dirPaths)
            {
                var uiPrefabs = LoadAllUIPrefabs(dirPath);
                foreach (var uiPrefab in uiPrefabs)
                {
                    var uiName = uiPrefab.name;
                    if (_uiPrefabs.ContainsKey(uiName))
                    {
                        Debug.LogError($"UIContainor.Initialized: UI {uiName} already exist");
                        continue;
                    }
                    _uiPrefabs.Add(uiName, uiPrefab);
                }
            }

            IsInitialized = true;
        }

        GameObject[] LoadAllUIPrefabs(string dirPath)
        {
            var uiPrefabs = Resources.LoadAll<GameObject>($"{_uiPath}/{dirPath}");
            uiPrefabs = Array.FindAll(uiPrefabs, uiPrefab => uiPrefab.GetComponent<UIObject.ViewBase>() != null);
            return uiPrefabs;
        }

        public GameObject GetUIPrefab(string name)
        {
            if (!_uiPrefabs.ContainsKey(name))
            {
                Debug.LogError($"UIContainor.GetUIPrefab: UI {name} not found");
                return null;
            }

            return _uiPrefabs[name];
        }

        public GameObject GetUICanvas()
        {
            var canvas = GameObject.FindWithTag(_uiCanvasName) ??
                throw new NullReferenceException($"UIContainor.GetUICanvas: canvas {_uiCanvasName} not found");

            return canvas;
        }

        public void OnBeforeSerialize()
        {
            UIName = new string[_uiPrefabs.Count];
            _uiPrefabs.Keys.CopyTo(UIName, 0);
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
