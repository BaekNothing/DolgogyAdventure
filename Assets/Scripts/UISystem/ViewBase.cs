using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public abstract class ViewBase : MonoBehaviour
    {
        public void Awake()
        {
            Initialized();
        }

        protected abstract void Initialized();

        protected void InitComponent(UIComponent component, UIComponent.UIComponentInitData data)
        {
            if (component == null)
                throw new System.ArgumentNullException($"{this.gameObject.name} ViewBase.InitComponent: component is null");

            component.Initialized(data);
        }

        public void TurnOff()
        {
            this.gameObject.SetActive(false);
        }

        public void TurnOn()
        {
            this.gameObject.SetActive(true);
        }
    }
}
