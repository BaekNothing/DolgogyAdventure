using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UISystem;
using TMPro;

namespace UISystem
{

    public class UIComponent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public struct UIComponentInitData
        {
            public Type type;
            public IViewData data;
            public Func<IViewData, ComponentStatus, ComponentStatus> evaluator;
            public Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> draw;

            public UIComponentInitData(Type type, IViewData data, Func<IViewData, ComponentStatus, ComponentStatus> evaluator,
                Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> draw)
            {
                if (type != typeof(TextMeshProUGUI) && type != typeof(Text) &&
                    type != typeof(Image) && type != typeof(Button))
                    throw new ArgumentException($"UIComponent.Init: type is not Text, Image or Button");

                if (data == null || evaluator == null || draw == null)
                    throw new ArgumentNullException($"UIComponent.Init: argument is null");

                this.type = type;
                this.data = data;
                this.evaluator = evaluator;
                this.draw = draw;
            }
        }

        public ComponentStatus Status = ComponentStatus.Enable;
        [SerializeField] UIBehaviour ComponentBody;
        IViewData ViewData;
        private Func<IViewData, ComponentStatus, ComponentStatus> Evaluator { get; set; } = (data, status) =>
            throw new NotImplementedException("Evaluator is not implemented");
        public Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> Draw { get; set; } = (body, data, status) =>
            throw new NotImplementedException($"Draw is not implemented");
        [SerializeField] UIComponentAction Action = new();

        public void Refresh()
        {
            if (ComponentBody == null || ViewData == null || Evaluator == null || Draw == null)
                throw new NullReferenceException($"{this.gameObject.name} UIComponent.Refresh: component is not initialized");

            Status = Evaluator(ViewData, Status);
            Draw(ComponentBody, ViewData, Status);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CoreSystem.DataContainor.IsInitialized) //= Game is not Ready
                throw new NullReferenceException
                    ($"{this.gameObject.name} UIComponent.OnPointerClick: DataContainor is not initialized");

            if (Action == null)
                throw new NullReferenceException($"{this.gameObject.name} UIComponent.OnPointerClick: Action is null");

#if UNITY_EDITOR
            Debug.Log($"{this.gameObject.name} UIComponent.OnPointerClick: {eventData.pointerCurrentRaycast.gameObject.name}");
#endif
            Action.Invoke();
        }

        public void Initialized(UIComponentInitData initData)
        {
            var body = transform.GetComponent(initData.type) as UIBehaviour;

            if (!body)
                throw new NullReferenceException($"{this.gameObject.name} UIComponent.Init: body is null");

            SetBody(body);
            SetData(initData.data);
            SetEvaluator(initData.evaluator);
            SetDraw(initData.draw);
        }

        public void SetBody(UIBehaviour body)
        {
            Debug.Assert(body != null);
            ComponentBody = body;
        }

        public void SetData(IViewData data)
        {
            Debug.Assert(data != null);

            if (ViewData != null && ViewData != data)
                ViewData.RemoveRefreshAction<UIComponent>(Refresh);
            else if (ViewData == data)
                return;

            data.AddRefreshAction<UIComponent>(Refresh);
            ViewData = data;
        }

        public void SetEvaluator(Func<IViewData, ComponentStatus, ComponentStatus> evaluator)
        {
            Debug.Assert(evaluator != null);
            Evaluator = evaluator;
        }

        public void SetDraw(Func<UIBehaviour, IViewData, ComponentStatus, UIBehaviour> draw)
        {
            Debug.Assert(draw != null);
            Draw = draw;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"> 해당 액션을 정의한 클래스의 출처 </typeparam>
        /// <param name="action"></param>
        public void SetAction<T>(Action action) where T : class
        {
            Debug.Assert(action != null);
            Action.SetAction<T>(action);
        }
    }
}
