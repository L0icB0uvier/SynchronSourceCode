using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "Layer Mask Variable", menuName = "ScriptableObjects/SingleValues/LayerMask", order = 0)]
    public class LayerMaskVariable : DescriptionBaseSO
    {
        public LayerMask Value;

        public void SetValue(LayerMask value)
        {
            Value = value;
        }

        public void SetValue(LayerMaskVariable value)
        {
            Value = value.Value;
        }
    }
}