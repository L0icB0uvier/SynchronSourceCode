using Camera;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Cutscenes
{
	public class CutsceneManager : MonoBehaviour
	{
		PlayableDirector m_PlayableDirector;

		public bool playCutsceneOnStart;
		[ShowIf("playCutsceneOnStart")]
		public PlayableAsset startCutscene;

		// Start is called before the first frame update
		private void Awake()
		{
			m_PlayableDirector = GetComponent<PlayableDirector>();
		}

		public void UpdateBinding(PlayableAsset playable)
		{
			var timeline = playable as TimelineAsset;
			foreach (var track in timeline.GetOutputTracks())
			{
				if (track.name == "Cinemachine Track")
				{
					//m_PlayableDirector.SetGenericBinding(track, CameraManager.Instance.Brain);
				}
			}
		}

		public void PlayCutScene(PlayableAsset playable)
		{
			UpdateBinding(playable);
			m_PlayableDirector.Play(playable);
		}

		public void PauseCutScene()
		{
			m_PlayableDirector.Pause();
		}

		public void ResumeCutScene()
		{
			m_PlayableDirector.Resume();
		}
	}
}
