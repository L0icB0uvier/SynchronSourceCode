using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "Vector2 Variable", menuName = "ScriptableObjects/SingleValues/Vector2")]
    public class Vector2Variable : DescriptionBaseSO
    {
        public Vector2 Value;

        public void SetValue(Vector2 value)
        {
            Value = value;
        }

        public void SetValue(Vector2Variable value)
        {
            Value = value.Value;
        }
    }
}