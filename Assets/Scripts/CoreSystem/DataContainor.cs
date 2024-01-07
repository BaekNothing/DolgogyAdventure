using UnityEngine;
using DataObject;

namespace CoreSystem
{
    public interface IDataContainor
    {
        DialogueViewData DialogueViewData { get; }
        T GetData<T>() where T : AData;
    }

    [CreateAssetMenu(fileName = "DataContainor", menuName = "ScriptableObjects/DataContainor", order = 1)]
    public class DataContainor : ScriptableObject, IDataContainor
    {
        [SerializeField] DialogueViewData _DialogueViewData;
        public DialogueViewData DialogueViewData { get => _DialogueViewData; }

        public void Initialized()
        {
            DialogueViewData.Initialized();
        }

        public T GetData<T>() where T : AData
        {
            return this.Get<T>();
        }

        T Get<T>() where T : AData
        {
            var type = typeof(T);
            if (type == typeof(DialogueViewData))
                return DialogueViewData as T;

            Debug.LogError($"DataContainor.Get: {type.Name} not found");
            return null;
        }
    }
}
