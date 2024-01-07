using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CoreSystem
{
    public interface IUIPrefabContainor
    {
        GameObject GetUIPrefab(string name);
    }

    [Serializable]
    public class UIPrefabContainor : IUIPrefabContainor
    {
        Dictionary<string, GameObject> _uiPrefabs = new();
        const string _uiPath = "UI";

        public void Initialized()
        {
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
    }
}
