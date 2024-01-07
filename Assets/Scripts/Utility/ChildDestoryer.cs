using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Utility
{
    public class ChildDestoryer : MonoBehaviour
    {
        public static int InitedDestoryerCount { get; private set; } = 0;
        public static ChildDestoryer[] CashedDestoryers { get; private set; } = null;
        public static bool IsAllDestoryerInited
        {
            get
            {
                CashedDestoryers ??= FindObjectsOfType<ChildDestoryer>();
                Logger.Log($"ChildDestoryer.IsAllDestoryerInited: {InitedDestoryerCount} / {CashedDestoryers.Length}");
                return InitedDestoryerCount >= CashedDestoryers.Length;
            }
        }

        public void Awake()
        {
            DestroyAllChildren();
        }

        async void DestroyAllChildren()
        {

            int childCount = this.transform.childCount;

            // 역순으로 제거해야 오류가 안남
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = this.transform.GetChild(i);
                Destroy(child.gameObject);
                while (child)
                    await UniTask.Yield();
            }

            InitedDestoryerCount++;
        }
    }
}
