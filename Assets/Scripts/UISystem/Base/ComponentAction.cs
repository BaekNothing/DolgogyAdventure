using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UISystem
{
    [Serializable]
    public class ComponentAction : ISerializationCallbackReceiver
    {
        public SortedList<string, Action> Actions { get; private set; }
        public string ActionsNames;

        public void SetAction<T>(Action action) where T : class
        {
            Debug.Assert(action != null);
            Actions ??= new SortedList<string, Action>();

            if (Actions.ContainsKey(typeof(T).Name))
                Actions[typeof(T).Name] += action;
            else
                Actions.Add(typeof(T).Name, action);
        }

        public void OnBeforeSerialize()
        {
            ActionsNames = string.Join("\n", Actions.Select(x => x.Key));
        }

        public void OnAfterDeserialize() { }
    }
}
