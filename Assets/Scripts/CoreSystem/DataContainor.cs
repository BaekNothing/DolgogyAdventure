using UnityEngine;
using DataObject;

namespace CoreSystem
{
    public interface IDataContainor
    {
        DialogueData DialogueData { get; }
        T GetData<T>() where T : AData;
    }

    [CreateAssetMenu(fileName = "DataContainor", menuName = "ScriptableObjects/DataContainor", order = 1)]
    public class DataContainor : ScriptableObject, IDataContainor
    {
        [SerializeField] DialogueData _dialogueData;
        public DialogueData DialogueData { get => _dialogueData; }

        public void Initialized()
        {
            DialogueData.Initialized();
        }

        public T GetData<T>() where T : AData
        {
            return this.Get<T>();
        }

        T Get<T>() where T : AData
        {
            var type = typeof(T);
            if (type == typeof(DialogueData))
                return DialogueData as T;

            Debug.LogError($"DataContainor.Get: {type.Name} not found");
            return null;
        }
    }
}
