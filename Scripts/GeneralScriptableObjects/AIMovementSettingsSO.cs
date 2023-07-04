using Sirenix.OdinInspector;
using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemiesMovementSettings", menuName = "ScriptableObjects/MovementSettings/AIMovementSettings", order = 0)]
    public class AIMovementSettingsSO : ScriptableObject
    {
        public bool useRandomSpeed;

        [HideIf("useRandomSpeed")]
        public int fixedSpeed;

        [ShowIf("useRandomSpeed")][MinMaxSlider(0, 20, true)]
        public Vector2Int randomSpeed = new Vector2Int(8, 12);
    }
}