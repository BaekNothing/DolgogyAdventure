using System;
using UnityEngine;
using System.Collections.Generic;

using Dolgoji.Resource;

namespace Dolgoji.UI
{
    // UI는 언제나 필요하다 = 굳이 싱글턴으로 만들 필요 없다.
    public static class UIUtility
    {
        static UIRenderer _currentUIRenderer;
        static Stack<UIRenderer> _uiRendererStack = new();
        static Dictionary<Type, IUIModel> _uiModelCache = new();

        public static void Show<T>() where T : UIRenderer
        {
            T renderer = LoadUIRenderer<T>();
            renderer.Initialize();
            renderer.Show();
            _uiRendererStack.Push(renderer);
            _currentUIRenderer = renderer;
        }

        public static void HideAt<T>() where T : UIRenderer
        {
            // 현재 렌더러가 T가 아니라면, T가 나올 때까지 스택에서 렌더러를 제거한다.
            while (_uiRendererStack.Count > 0 && _currentUIRenderer?.GetType() != typeof(T))
            {
                CloseTop();
            }

            // T가 나왔다면, T를 제거한다.
            if (_uiRendererStack.Count > 0)
            {
                CloseTop();
            }
        }

        public static void CloseTop()
        {
            if (!_currentUIRenderer || _uiRendererStack.Count == 0)
            {
                return;
            }

            UIRenderer renderer = _uiRendererStack.Pop();
            renderer.Hide();
            _currentUIRenderer = _uiRendererStack.Count > 0 ? _uiRendererStack.Peek() : null;
        }

        static T LoadUIRenderer<T>() where T : UIRenderer
        {
            T renderer = ResourceUtility.Instantiate<T>(
                $"{UIConsts.UI_RENDERER_PATH}/{typeof(T).Name}", UIConsts.UIRootCanvas.gameObject);
            return renderer;
        }

        public static T GetUIModel<T>() where T : UIModel
        {
            if (_uiModelCache.ContainsKey(typeof(T)))
            {
                return (T)_uiModelCache[typeof(T)];
            }

            T uiModel = ResourceUtility.Load<T>($"{UIConsts.UI_MODEL_PATH}/{typeof(T).Name}");
            return uiModel;
        }
    }
}
