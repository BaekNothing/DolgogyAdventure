using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFSW.QC;

namespace CoreSystem
{
    [Serializable]
    struct InputActionData
    {
        public KeyCode KeyCode;
        public Dictionary<GameObject, Action> Actions;

        public InputActionData(KeyCode keyCode)
        {
            this.KeyCode = keyCode;
            this.Actions = new();
        }

        public readonly void SetAction(Action action, GameObject source)
        {
            if (Actions.ContainsKey(source))
            {
                Actions[source] -= action;
                Actions[source] += action;
            }
            else
            {
                Actions.Add(source, action);
            }
        }

        public readonly void Invoke()
        {
            foreach (var eachAction in Actions)
            {
                if (eachAction.Key)
                {
                    eachAction.Value?.Invoke();
                }
            }

            Clear();
        }

        readonly void Clear()
        {
            var targets = Actions.Where((eachAction) => !eachAction.Key);
            foreach (var target in targets)
                Actions.Remove(target.Key);
        }
    }

    public interface IInputContainor
    {
        public enum InputType
        {
            Hold,
            DownOnce
        }

        public struct SetInputActionData
        {
            public InputType type;
            public KeyCode key;
            public Action action;
            public GameObject source;

            public SetInputActionData
                (InputType type, KeyCode key, Action action, GameObject source)
            {
                this.type = type;
                this.key = key;
                this.action = action;
                this.source = source;
            }
        }

        public void SetInputAction(SetInputActionData data);
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

        public void Initialized(GameObject systemRoot)
        {
            SetInputAction(
                new IInputContainor.SetInputActionData
                (IInputContainor.InputType.DownOnce, KeyCode.BackQuote, () =>
                {

                    if (QuantumConsole.Instance.IsActive)
                        QuantumConsole.Instance.Deactivate();
                    else
                        QuantumConsole.Instance.Activate();
                }, systemRoot)
            );
        }

        public void Invoke(KeyCode key, IInputContainor.InputType type)
        {
            if (_actionDatas[type].ContainsKey(key))
                _actionDatas[type][key].Invoke();
        }


        public void SetInputAction(IInputContainor.SetInputActionData data)
        {
            if (!_actionDatas[data.type].ContainsKey(data.key))
            {
                var actionData = new InputActionData(data.key);
                _actionDatas[data.type].Add(data.key, actionData);
            }

            _actionDatas[data.type][data.key].SetAction(data.action, data.source);
        }
    }
}
