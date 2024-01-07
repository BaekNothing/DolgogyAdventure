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
        public void Pop<T>() where T : ViewBase;
        public ViewBase Pick();
    }

    [Serializable]
    public class UIViewStackContainor : IUIViewStackContainor
    {
        readonly UIPrefabContainor _uiContainor = new();
        const string _uiCanvasName = "UICanvas";

        public List<ViewBase> _viewStack = new();
        static readonly Dictionary<Type, ViewBase> _cashedView = new();

        public string FocusViewName;

        public void Initialized()
        {
            _viewStack.Clear();
            _cashedView.Clear();
            _uiContainor.Initialized();
        }

        public void Focus<T>() where T : ViewBase
        {
            Utility.Logger.Log($"UIViewStackContainor.Focus: {typeof(T).Name}");
            var view = GetView<T>();
            if (view != null)
            {
                while (PickView() != view)
                {
                    PopView();
                }

                view.OnResume();
                view.SetTop();
                view.OnEnter();
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

        public void Pop<T>() where T : ViewBase
        {
            Utility.Logger.Log($"UIViewStackContainor.Pop: {typeof(T).Name}");
            var topView = PickView();
            if (topView != null && topView.GetType() == typeof(T))
            {
                PopView();
            }
            else
            {
                Debug.LogError($"View {typeof(T).Name} not found");
            }
        }

        public ViewBase Pick()
        {
            return PickView();
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

                var view = UnityEngine.Object.Instantiate(prefab, targetCanvas.transform).GetComponent<T>() ??
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


        void PushView(ViewBase view)
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
            view.OnResume();
            view.SetTop();
            view.OnEnter();
        }

        void PopView()
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

        ViewBase PickView()
        {
            if (_viewStack.Count > 0)
            {
                return _viewStack[^1];
            }
            return null;
        }
    }
}
