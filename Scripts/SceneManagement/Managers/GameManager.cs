using System;
using System.Collections;
using System.Linq;
using Camera;
using GeneralScriptableObjects;
using SceneManagement.LevelManagement;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement.Managers
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private LocationSO currentSceneSO;
		[SerializeField] private PathSO lastPath;

		
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
