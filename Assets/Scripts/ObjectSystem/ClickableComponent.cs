using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ObjectSystem
{
    public class ClickableComponent : MonoBehaviour
    {
        Action _action;

        public void SetAction(Action action)
        {
            _action = action;
        }

        public void OnMouseDown()
        {
            _action?.Invoke();
        }
    }
}
