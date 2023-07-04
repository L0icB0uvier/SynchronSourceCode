using SceneManagement.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
	public class SceneDataLoader : MonoBehaviour
	{
		private void Awake()
		{
			LoadSceneDatas();
		}

		private void Start()
		{
			if (!ES3.FileExists("SavedData/Data" + SceneManager.GetActiveScene().name + ".es3"))
			{
				//GameManager.Instance.SaveSceneData();
			}
		}

		[Button]
		public void LoadSceneDatas()
		{
			var settings = new ES3SerializableSettings {location = ES3.Location.File, path = "SavedData"};
			ES3AutoSaveMgr.Current.settings = settings;
			if (ES3.FileExists("SavedData/Data" + SceneManager.GetActiveScene().name + ".es3"))
			{
				ES3AutoSaveMgr.Current.Load();
			}
		}
	}
}
