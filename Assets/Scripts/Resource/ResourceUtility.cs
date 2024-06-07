using System.Collections.Generic;
using UnityEngine;

namespace Dolgoji.Resource
{
    public static class ResourceUtility
    {
        static Dictionary<string, Object> _resourceCache = new();

        static Dictionary<string, GameObject> _instantiatedCache = new();

        public static T Load<T>(string path) where T : Object
        {
            if (_resourceCache.ContainsKey(path))
            {
                return _resourceCache[path] as T;
            }
            T resource = Resources.Load<T>(path);
            _resourceCache.Add(path, resource);

            return resource;
        }

        public static T Instantiate<T>(string path, GameObject parent) where T : MonoBehaviour
        {
            if (_instantiatedCache.ContainsKey(path))
            {
                return _instantiatedCache[path].GetComponent<T>();
            }

            T resource = Load<T>(path);
            Debug.Log($"{resource}");
            GameObject instantiated = GameObject.Instantiate(resource.gameObject, parent.transform);
            _instantiatedCache.Add(path, instantiated);

            return instantiated.GetComponent<T>();
        }
    }
}
