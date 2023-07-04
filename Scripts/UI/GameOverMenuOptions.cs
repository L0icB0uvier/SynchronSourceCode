using UnityEngine;

namespace UI
{
	public class GameOverMenuOptions : MonoBehaviour
	{
		public void RestartMission()
		{

		}

		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
		}
	}
}
