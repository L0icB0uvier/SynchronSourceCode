using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "Bool Variable", menuName = "ScriptableObjects/SingleValues/Bool")]
    public class BoolVariable : DescriptionBaseSO
    {
        public bool Value;

        public void SetValue(bool value)
        {
            Value = value;
        }

        public void SetValue(BoolVariable value)
        {
            Value = value.Value;
        }
    }
}