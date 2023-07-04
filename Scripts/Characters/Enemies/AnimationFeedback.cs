using UnityEngine;
using UnityEngine.Events;

namespace Characters.Ennemies
{
	public class AnimationFeedback : MonoBehaviour
	{
		public UnityEvent OnDeathAnimationOver;

		public UnityEvent OnExitRestAnimationOver;

		public void DeathAnimationOver()
		{
			OnDeathAnimationOver.Invoke();
		}

		public void ExitRestAnimationOver()
		{
			OnExitRestAnimationOver.Invoke();
		}
	}
}
