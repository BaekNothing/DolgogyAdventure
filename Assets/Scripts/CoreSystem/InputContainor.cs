using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

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
        public enum InputType
        {
            Hold,
            DownOnce
        }

        public void SetAction(InputType tytpe, KeyCode key, Action action);
    }

    [CreateAssetMenu(fileName = "InputContainor", menuName = "ScriptableObjects/InputContainor", order = 1)]
    public class InputContainor : ScriptableObject, IInputContainor
    {

        readonly Dictionary<IInputContainor.InputType, Dictionary<KeyCode, InputActionData>> _actionDatas
        = new()
        {
            {IInputContainor.InputType.Hold, new Dictionary<KeyCode, InputActionData>()},
            {IInputContainor.InputType.DownOnce, new Dictionary<KeyCode, InputActionData>()},
        };

        public void Initialized()
        {
            SetAction(IInputContainor.InputType.DownOnce, KeyCode.BackQuote, () =>
            {

                if (QuantumConsole.Instance.IsActive)
                    QuantumConsole.Instance.Deactivate();
                else
                    QuantumConsole.Instance.Activate();
            });
        }

        public void Invoke(KeyCode key, IInputContainor.InputType type)
        {
            if (_actionDatas[type].ContainsKey(key))
                _actionDatas[type][key].Invoke();
        }

        public void SetAction(IInputContainor.InputType type, KeyCode key, Action action)
        {
            if (!_actionDatas[type].ContainsKey(key))
            {
                var actionData = new InputActionData(key);
                _actionDatas[type].Add(key, actionData);
            }

            _actionDatas[type][key].SetAction(action);
        }
    }
}
