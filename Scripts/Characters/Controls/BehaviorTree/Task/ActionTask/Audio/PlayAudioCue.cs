using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using GeneralScriptableObjects.Events;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Audio
{
	[TaskCategory("Audio")]
	public class PlayAudioCue : Action
	{
		public SharedAIController AIController;
		public AudioCueEventChannelSO _sfxEventChannel;
		public AudioConfigurationSO _audioConfig;
		public AudioCueSO _audioCue;

		public override TaskStatus OnUpdate()
		{
			_sfxEventChannel.RaisePlayEvent(_audioCue, _audioConfig, AIController.Value.transform.position);
			return TaskStatus.Success;
		}
	}
}