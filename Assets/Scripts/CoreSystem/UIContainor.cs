using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIObject;

namespace CoreSystem
{
    public interface IUIContainor : IUIViewStackContainor
    {

    }

    [CreateAssetMenu(fileName = "UIContainor", menuName = "ScriptableObjects/UIContainor", order = 1)]
    public class UIContainor : ScriptableObject, IUIContainor
    {
        [SerializeField] UIViewStackContainor _uiViewStackContainor = new();

        public void Initialized()
        {
            _uiViewStackContainor.Initialized();
            Focus<RootMenuView>();
        }

        public void Focus<T>() where T : ViewBase
        {
            _uiViewStackContainor.Focus<T>();
        }
    }
}
