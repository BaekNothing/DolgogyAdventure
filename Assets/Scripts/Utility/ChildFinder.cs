using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ChildFinder
    {
        public static List<T> GetAll<T>(Transform parent) where T : Component
        {
            List<T> result = new();

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                var component = child.GetComponent<T>();
                if (component != null)
                    result.Add(component);
                if (child.childCount > 0)
                    result.AddRange(GetAll<T>(child));
            }

            return result;
        }
    }
}
