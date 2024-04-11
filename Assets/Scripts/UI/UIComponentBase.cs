using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dolgoji.UIComponent
{
    public interface IUIComponentBase
    {
        public interface IUIComponentData
        {
            public T GetData<T>() where T : Object;
        }

        public void Init<T>(T data) where T : IUIComponentData;
        public void Show();
        public void Hide();
    }

    public abstract class UIComponentBase : MonoBehaviour, IUIComponentBase
    {
        public abstract void Init<T>(T data) where T : IUIComponentBase.IUIComponentData;
        public abstract void Show();
        public abstract void Hide();
    }
}
