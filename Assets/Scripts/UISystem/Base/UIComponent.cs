using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UISystem
{
    public class UIComponent<TComponent, TData> : Component where TComponent : Component where TData : IViewData
    {
        public enum ComponentType
        {
            Button,
            Text,
            Image,
        }

        public ComponentType Type;
        public TComponent ComponentBody { get; set; }
        public ComponentStatus Status { get; set; }

        public TData ViewData { get; set; }
        public UnityAction OnClick { get; set; } = () =>
            throw new NotImplementedException("OnClick is not implemented");
        private Func<TData, ComponentStatus, ComponentStatus> Option { get; set; } = (data, status) =>
            throw new NotImplementedException("Option is not implemented");
        private Action<TComponent, ComponentStatus> Draw { get; set; } = (status, type) =>
            throw new NotImplementedException("Draw is not implemented");

        public void SetData(TData data)
        {
            Debug.Assert(data != null);
            ViewData = data;
        }

        public void SetOnClick(UnityAction onClick)
        {
            Debug.Assert(onClick != null);
            Debug.Assert(ComponentBody is Button);

            OnClick = onClick;
            if (ComponentBody is Button button)
                button.onClick.AddListener(OnClick);
        }

        public void SetOption(Func<TData, ComponentStatus, ComponentStatus> option)
        {
            Debug.Assert(option != null);
            Option = option;
        }

        public void SetDraw(Action<TComponent, ComponentStatus> draw)
        {
            Debug.Assert(draw != null);
            Draw = draw;
        }

        public void Refresh()
        {
            Status = Option(ViewData, Status);
            Draw(ComponentBody, Status);
        }

        public void Init(ref TData data,
            Func<TData, ComponentStatus, ComponentStatus> option, Action<TComponent, ComponentStatus> draw)
        {
            ComponentBody = gameObject.GetComponent<TComponent>();
            Debug.Assert(ComponentBody != null);
            Type = SetComponentType(ComponentBody);

            SetData(data);
            SetOption(option);
            SetDraw(draw);
        }

        ComponentType SetComponentType(Component component)
        {
            if (component is Button)
                return ComponentType.Button;
            else if (component is Text)
                return ComponentType.Text;
            else if (component is Image)
                return ComponentType.Image;
            else
                throw new NotImplementedException("ComponentType is not implemented");
        }
    }
}
