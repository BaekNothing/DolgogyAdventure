using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UIObject
{
    [Serializable]
    public class UIComponentAction : ISerializationCallbackReceiver
    {
        public SortedList<string, Action> Actions { get; private set; } = new();
        public string ActionsNames;

        public void Invoke()
        {
            Debug.Assert(Actions != null);
            foreach (var action in Actions)
                action.Value?.Invoke();
        }

        public void Invoke<T>() where T : class
        {
            Debug.Assert(Actions != null);
            Debug.Assert(Actions.ContainsKey(typeof(T).Name));
            Actions[typeof(T).Name]?.Invoke();
        }

        public void SetAction<T>(Action action) where T : class
        {
            Debug.Assert(action != null);
            Actions ??= new SortedList<string, Action>();

            string typeName = typeof(T).Name;

            if (Actions.ContainsKey(typeName))
            {
                // Prevent duplicate actions
                Actions[typeName] -= action;
                Actions[typeName] += action;
            }
            else
                Actions.Add(typeName, action);
        }

        public void RemoveAction<T>(Action action) where T : class
        {
            Debug.Assert(action != null);
            Debug.Assert(Actions != null);

            string typeName = typeof(T).Name;

            if (Actions.ContainsKey(typeName))
                Actions[typeName] -= action;
        }

        public void OnBeforeSerialize()
        {
            ActionsNames = string.Join("\n", Actions.Select(x => x.Key + " : " + x.Value.Method.Name));
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
