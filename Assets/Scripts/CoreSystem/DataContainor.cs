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
        readonly Dictionary<string, AData> _dataList = new();
        const string _uiPath = "UI";

        public void Initialized()
        {
            LoadAllData();
        }

        public void LoadAllData()
        {
            var DirInfo = new DirectoryInfo($"{Application.dataPath}/Resources/{_uiPath}") ??
                throw new NullReferenceException($"UIContainor.Initialized: {_uiPath} not found");

            _dataList.Clear();
            var dirPaths = DirInfo.GetDirectories().Select(dir => dir.Name).ToArray();
            foreach (var dirPath in dirPaths)
            {
                var dataObjects = LoadAllDataObjects(dirPath);
                foreach (var dataObject in dataObjects)
                {
                    var objName = dataObject.name;
                    if (_dataList.ContainsKey(objName))
                    {
                        Debug.LogError($"UIContainor.Initialized: UI {objName} already exist");
                        continue;
                    }
                    _dataList.Add(objName, dataObject);
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
            if (_dataList.ContainsKey(type.Name))
            {
                return _dataList[type.Name] as T;
            }
            else
            {
                Debug.LogError($"DataContainor.Get: {type.Name} not found");
                return null;
            }
        }
    }
}
