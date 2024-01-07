using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIObject
{
    public abstract class ViewBase : MonoBehaviour
    {
        public const string TargetCanvasName = "UICanvas";

        public void Start()
        {
            Initialized();
        }

        public bool IsPaused { get; private set; } = false;

        protected abstract void Initialized();

        protected void InitComponent(UIComponent component, UIComponent.UIComponentInitData data)
        {
            if (component == null)
                throw new System.ArgumentNullException($"{this.gameObject.name} ViewBase.InitComponent: component is null");

            component.Initialized(data);
        }

        public virtual void OnEnter()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void OnExit()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void OnPause()
        {
            IsPaused = true;
        }

        public virtual void OnResume()
        {
            IsPaused = false;
        }
    }
}
