using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    public class CoreSystem : MonoBehaviour
    {
        [SerializeField] DataContainor _dataContainor;

        private void Awake()
        {
            _dataContainor.Initialized();
        }
    }
}
