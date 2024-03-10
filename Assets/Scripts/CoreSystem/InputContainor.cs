using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem
{
    [Serializable]
    struct InputActionData
    {
        public KeyCode KeyCode;
        public List<Action> actions;

        public InputActionData(KeyCode keyCode)
        {
            this.KeyCode = keyCode;
            this.actions = new List<Action>();
        }

        public readonly void SetAction(Action action)
        {
            if (!actions.Contains(action))
                actions.Add(action);
        }

        public readonly void Invoke()
        {
            actions.ForEach((action) => action?.Invoke());
        }
    }

    public interface IInputContainor
    {
        public void SetAction(KeyCode key, Action action);
    }

    [CreateAssetMenu(fileName = "InputContainor", menuName = "ScriptableObjects/InputContainor", order = 1)]
    public class InputContainor : ScriptableObject, IInputContainor
    {
        Dictionary<KeyCode, InputActionData> _actionDatas = new();
#if UNITY_EDITOR
        List<InputActionData> _dataInspectorShower = new();
#endif
        public void Invoke(KeyCode key)
        {
            if (_actionDatas.ContainsKey(key))
                _actionDatas[key].Invoke();
        }

        public void SetAction(KeyCode key, Action action)
        {
            if (!_actionDatas.ContainsKey(key))
                _actionDatas.Add(key, new InputActionData(key));

            _actionDatas[key].SetAction(action);
        }
    }
}
