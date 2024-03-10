using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataObject;

namespace CoreSystem
{
    public interface IDataContainor
    {
        T GetData<T>() where T : AData;
    }

    [CreateAssetMenu(fileName = "DataContainor", menuName = "ScriptableObjects/DataContainor", order = 1)]
    public class DataContainor : ScriptableObject, IDataContainor
    {
        readonly Dictionary<string, AData> _dataDict = new();
#if UNITY_EDITOR
        [SerializeField] List<AData> _dataInspectorShower = new();
#endif

        const string _uiPath = "UI";

        public void Initialized()
        {
            LoadAllData();
        }

        public void LoadAllData()
        {
            var DirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/{_uiPath}") ??
                throw new NullReferenceException($"UIContainor.Initialized: {_uiPath} not found");

            _dataDict.Clear();
#if UNITY_EDITOR
            _dataInspectorShower.Clear();
#endif
            var dirPaths = DirInfo.GetDirectories().Select(dir => dir.Name).ToArray();
            foreach (var dirPath in dirPaths)
            {
                var dataObjects = LoadAllDataObjects(dirPath);
                foreach (var dataObject in dataObjects)
                {
                    var objName = dataObject.name;
                    if (_dataDict.ContainsKey(objName))
                    {
                        Debug.LogError($"UIContainor.Initialized: UI {objName} already exist");
                        continue;
                    }
                    _dataDict.Add(objName, dataObject);
#if UNITY_EDITOR
                    _dataInspectorShower.Add(dataObject);
#endif
                }
            }
        }

        AData[] LoadAllDataObjects(string dirPath)
        {
            var data = Resources.LoadAll<AData>($"{_uiPath}/{dirPath}");
            return data;
        }

        public T GetData<T>() where T : AData
        {
            return this.Get<T>();
        }

        T Get<T>() where T : AData
        {
            var type = typeof(T);
            if (_dataDict.ContainsKey(type.Name))
            {
                return _dataDict[type.Name] as T;
            }
            else
            {
                Debug.LogError($"DataContainor.Get: {type.Name} not found");
                return null;
            }
        }
    }
}
