using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "Bool Variable", menuName = "ScriptableObjects/SingleValues/BoolNotifyChange")]
    public class BoolVariableNotifyChange : DescriptionBaseSO
    {
        public bool Value;
        public UnityAction onValueChanged;

        public void SetValue(bool value)
        {
            Value = value;
            onValueChanged?.Invoke();
        }

        public void SetValue(BoolVariable value)
        {
            Value = value.Value;
        }
    }
}