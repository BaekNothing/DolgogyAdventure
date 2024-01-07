using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UISystem;

namespace CoreSystem
{
    public interface IUIViewStackContainor
    {
        public static void ShowView<T>() where T : ViewBase { }
        public static void HideView<T>() where T : ViewBase { }
    }

    [CreateAssetMenu(fileName = "UIViewStackContainor", menuName = "ScriptableObjects/UIViewStackContainor", order = 1)]
    public class UIViewStackContainor : ScriptableObject, ISerializationCallbackReceiver, IUIViewStackContainor
    {
        static readonly List<ViewBase> _viewStack = new();
        public string ViewStackName;

        public void Initialized()
        {
            _viewStack.Clear();
            ShowView<RootMenuView>();
        }

        public static void ShowView<T>() where T : ViewBase
        {
            var view = GetView<T>();
            if (view == null)
            {
                view = LoadView<T>();
                if (view == null)
                {
                    Debug.LogError($"View {typeof(T).Name} not found");
                    return;
                }
            }

            if (_viewStack.Contains(view))
            {
                Debug.LogError($"View {typeof(T).Name} already in stack");
                return;
            }

            PushView(view);
        }

        public static void HideView<T>() where T : ViewBase
        {
            var view = GetView<T>();
            if (view == null)
            {
                Debug.LogError($"View {typeof(T).Name} not found");
                return;
            }

            if (!_viewStack.Contains(view))
            {
                Debug.LogError($"View {typeof(T).Name} not in stack");
                return;
            }

            PopView();
        }

        static T GetView<T>() where T : ViewBase
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

        static T LoadView<T>() where T : ViewBase
        {
            Utility.Logger.Log($"LoadView Start", Utility.Logger.Importance.Warning);
            var prefab = SystemRoot.UIContainor.GetUIPrefab(typeof(T).Name) ??
                throw new System.NullReferenceException($"View {typeof(T).Name} not found");

            Utility.Logger.Log($"LoadView: {prefab.name}", Utility.Logger.Importance.Warning);

            var targetCanvas = SystemRoot.UIContainor.GetUICanvas() ??
                throw new System.NullReferenceException($"Canvas not found");

            Utility.Logger.Log($"Canvas: {targetCanvas.name}");

            var view = Instantiate(prefab, targetCanvas.transform);
            Utility.Logger.Log($"Instantiate: {view.name}");
            return view.GetComponent<T>();
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
            view.OnExit();
            _viewStack.RemoveAt(_viewStack.Count - 1);

            if (_viewStack.Count > 0)
            {
                _viewStack[^1].OnResume();
            }
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
