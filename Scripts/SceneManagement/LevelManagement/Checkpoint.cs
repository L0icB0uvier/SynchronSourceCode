using Characters.Controls.Controllers.PlayerControllers;
using Characters.Controls.Controllers.PlayerControllers.Hicks;
using Characters.Controls.Controllers.PlayerControllers.Skullface;
using GeneralEnums;
using RuntimeAnchors;
using SceneManagement.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneManagement.LevelManagement
{
	public class Checkpoint : MonoBehaviour
	{
		public EPlayerCharacterType activatedBy;
		
		public Transform hicksRespawnLocation;
		public Transform skullfaceRespawnLocation;
		
		[SerializeField] private EIsometricCardinal4DiagonalDirection checkpointFacingDirection;
		public EIsometricCardinal4DiagonalDirection CheckpointLookingDirection => checkpointFacingDirection;

		[FormerlySerializedAs("checkPointStorage")] [SerializeField] private CheckpointStorageSO _checkPointStorageSo;
		[SerializeField] private CheckpointSO checkpoint;
		public CheckpointSO CheckpointPath => checkpoint;
		
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag(activatedBy.ToString()))
			{
				_checkPointStorageSo.lastCheckPoint = checkpoint;
			}
		}
	}
}
