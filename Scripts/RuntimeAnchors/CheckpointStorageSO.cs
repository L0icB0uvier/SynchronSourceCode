using GeneralScriptableObjects;
using UnityEngine;

namespace RuntimeAnchors
{
    [CreateAssetMenu(fileName = "CheckpointStorage", menuName = "Gameplay/Checkpoint Storage")]
    public class CheckpointStorageSO : DescriptionBaseSO
    {
        [Space] public CheckpointSO lastCheckPoint;
    }
}