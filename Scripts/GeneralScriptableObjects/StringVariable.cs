using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "String Variable", menuName = "ScriptableObjects/SingleValues/String")]
    public class StringVariable : DescriptionBaseSO
    {
        public string Value;

        public void SetValue(string value)
        {
            Value = value;
        }
    }
}