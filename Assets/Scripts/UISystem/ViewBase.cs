using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public abstract class ViewBase : MonoBehaviour
    {
        protected List<UIComponent> Components { get; set; } = new();
        [SerializeField] bool IsInitialized = false;

        public void Refresh()
        {
            if (!this.gameObject.activeSelf)
                return;

            if (!IsInitialized)
                throw new System.Exception($"{this.gameObject.name} ViewBase.Refresh: page is not initialized");

            foreach (var component in Components)
                component.Refresh();
        }

        public void Awake()
        {
            UIManager.RegisterView(this);
            Initialized();
            IsInitialized = true;

            Refresh();
        }

        protected abstract void Initialized();

        protected void InitComponent(UIComponent component, UIComponent.UIComponentInitData data)
        {
            if (component == null)
                throw new System.ArgumentNullException($"{this.gameObject.name} ViewBase.InitComponent: component is null");

            if (Components.Contains(component))
                throw new System.Exception($"{this.gameObject.name} ViewBase.InitComponent: component is already initialized");

            Components.Add(component);
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
