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
                return InitedDestoryerCount == CashedDestoryers.Length;
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
                Destroy(this.transform.GetChild(i).gameObject);
                while (this.transform.childCount > i)
                    await UniTask.Yield();
            }

            InitedDestoryerCount++;
        }
    }
}
