using System.Collections;
using Characters.Controls.Controllers.PlayerControllers.Hicks;
using Characters.Controls.Controllers.PlayerControllers.Skullface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Cutscenes
{
	public class CutSceneTrigger : MonoBehaviour
	{
		public string[] triggeringTags = new string[0];

		public bool delayCutscenePreparation;

		[ShowIf("delayCutscenePreparation")]
		[Indent]
		public float delay;

		public CutsceneManager cutsceneManager;
		public PlayableAsset cutscene;

		HicksPlayerController m_Hicks;
		SkullfacePlayerController m_skullfacePlayerController;

		public bool hicksMovement;

		[ShowIf("hicksMovement")]
		[Indent]
		public Vector2 hicksCutsceneStartLocation;

		bool m_HicksAtStartLocation;

		public IOnTriggerDroneBehavior DroneBehavior = IOnTriggerDroneBehavior.DisableMovement;

		[ShowIf("ShowDroneCutsceneStartLocation")]
		[Indent]
		public Vector2 droneCutSceneStartLocation;
		bool m_DroneAtStartLocation;

		public UnityEvent CutsceneTriggered;

		bool ShowDroneCutsceneStartLocation()
		{
			if (DroneBehavior == IOnTriggerDroneBehavior.MoveToLocation)
			{
				return true;
			}

			else return false;
		}

		private void Awake()
		{
			m_Hicks = FindObjectOfType<HicksPlayerController>();
			m_skullfacePlayerController = FindObjectOfType<SkullfacePlayerController>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			foreach(string tag in triggeringTags)
			{
				if(collision.tag == tag)
				{
					CutsceneTriggered.Invoke();

					if (delayCutscenePreparation)
					{
						StartCoroutine(DelayCutScenePreparation(delay));
					}

					else
					{
						PrepareCutScene();
					}
				
					return;
				}
			}
		}

		IEnumerator DelayCutScenePreparation(float delay)
		{
			yield return new WaitForSeconds(delay);
			PrepareCutScene();
		}

		private void PrepareCutScene()
		{
			if(DroneBehavior != IOnTriggerDroneBehavior.MoveToLocation && !hicksMovement)
			{
				cutsceneManager.PlayCutScene(cutscene);
				gameObject.SetActive(false);
			}

			else
			{
				switch (DroneBehavior)
				{
					case IOnTriggerDroneBehavior.DisableMovement:
						//m_skullfacePlayerController.DisableControls();
						break;
					case IOnTriggerDroneBehavior.FollowHicks:
						//m_PlayerDrone.RecallDrone(false);
						break;
					case IOnTriggerDroneBehavior.MoveToLocation:
						//m_skullfacePlayerController.MoveByAI(droneCutSceneStartLocation, OnDroneAtCutseneLocation);
						break;
				}

				if (hicksMovement)
				{
					//m_Hicks.MoveByAI(hicksCutsceneStartLocation, OnHicksAtCutscenePos);
				}
			}	
		}

		public void OnHicksAtCutscenePos()
		{
			m_HicksAtStartLocation = true;

			if(DroneBehavior == IOnTriggerDroneBehavior.MoveToLocation && !m_DroneAtStartLocation)
			{
				return;
			}

			cutsceneManager.PlayCutScene(cutscene);
			gameObject.SetActive(false);		
		}

		public void OnDroneAtCutseneLocation()
		{
			m_DroneAtStartLocation = true;

			if(hicksMovement && !m_HicksAtStartLocation)
			{
				return;
			}

			cutsceneManager.PlayCutScene(cutscene);
			gameObject.SetActive(false);
		}

		public enum IOnTriggerDroneBehavior { FollowHicks, DisableMovement, MoveToLocation }
	}
}
