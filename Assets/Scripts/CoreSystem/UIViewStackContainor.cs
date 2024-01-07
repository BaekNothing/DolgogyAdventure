using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UIObject;

namespace CoreSystem
{
    public interface IUIViewStackContainor
    {
        public void Focus<T>() where T : ViewBase;
    }

    [CreateAssetMenu(fileName = "UIViewStackContainor", menuName = "ScriptableObjects/UIViewStackContainor", order = 1)]
    public class UIViewStackContainor : ScriptableObject, ISerializationCallbackReceiver, IUIViewStackContainor
    {
        [SerializeField] UIContainor _uiContainor;
        public string FocusViewName;
        static readonly List<ViewBase> _viewStack = new();
        static readonly Dictionary<Type, ViewBase> _cashedView = new();
        public string ViewStackName;
        const string _uiCanvasName = "UICanvas";

        public void Initialized()
        {
            _uiContainor.Initialized();
            _viewStack.Clear();
            _cashedView.Clear();

            Focus<RootMenuView>();
            //Focus<DialogueView>();
        }

        public void Focus<T>() where T : ViewBase
        {
            Utility.Logger.Log($"UIViewStackContainor.Focus: {typeof(T).Name}");
            var view = GetView<T>();
            if (view != null)
            {
                while (PickView<T>() != view)
                {
                    PopView();
                }
                view.OnEnter();
                view.OnResume();
                view.SetTop();
            }
            else
            {
                view = LoadView<T>();
                if (view == null)
                {
                    Debug.LogError($"View {typeof(T).Name} not found");
                    return;
                }
                PushView(view);
            }

            FocusViewName = view.GetType().Name;
        }

        T GetView<T>() where T : ViewBase
        {
            if (_viewStack.Count > 0)
            {
                var view = _viewStack.Find(v => v.GetType() == typeof(T));
                if (view != null)
                {
                    return view as T;
                }
            }
            return null;
        }

        T LoadView<T>() where T : ViewBase
        {
            if (_cashedView.ContainsKey(typeof(T)))
            {
                return _cashedView[typeof(T)] as T;
            }
            else
            {
                var prefab = _uiContainor.GetUIPrefab(typeof(T).Name) ??
                throw new System.NullReferenceException($"View {typeof(T).Name} not found");

                var targetCanvas = GetUICanvas() ??
                    throw new System.NullReferenceException($"Canvas not found");

                var view = Instantiate(prefab, targetCanvas.transform).GetComponent<T>() ??
                    throw new System.NullReferenceException($"View {typeof(T).Name} not found");

                view.Initialized();
                _cashedView.Add(typeof(T), view);
                return view;
            }
        }

        public GameObject GetUICanvas()
        {
            var canvas = GameObject.FindWithTag(_uiCanvasName) ??
                throw new NullReferenceException($"UIContainor.GetUICanvas: canvas {_uiCanvasName} not found");

            return canvas;
        }


        static void PushView(ViewBase view)
        {
            if (_viewStack.Contains(view))
            {
                Debug.LogError("View already in stack");
                return;
            }

            if (_viewStack.Count > 0)
            {
                _viewStack[^1].OnPause();
            }

            _viewStack.Add(view);
            view.SetTop();
            view.OnEnter();
        }

        static void PopView()
        {
            if (_viewStack.Count == 0)
            {
                Debug.LogError("View stack is empty");
                return;
            }

            var view = _viewStack[^1];
            view.OnPause();
            view.OnExit();
            _viewStack.RemoveAt(_viewStack.Count - 1);

            if (_viewStack.Count > 0)
            {
                _viewStack[^1].OnResume();
            }
        }

        static T PickView<T>() where T : ViewBase
        {
            if (_viewStack.Count > 0)
            {
                var view = _viewStack.Find(v => v.GetType() == typeof(T));
                if (view != null)
                {
                    return view as T;
                }
            }
            return null;
        }

        public void OnBeforeSerialize()
        {
            ViewStackName = string.Join("\n", _viewStack.Select(x => x.GetType().Name));
        }

        public void OnAfterDeserialize()
        {

        }

    }
}
