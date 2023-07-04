using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.PoweredObjects.AutonomousPoweredObjects
{
	public class AirVent : MonoBehaviour
	{
		[FoldoutGroup("Components References")]
		public AreaEffector2D areaEffector;

		[FoldoutGroup("Components References")]
		public ParticleSystem particuleSystem;

		[FoldoutGroup("Settings")]
		public bool startOn;

		[FoldoutGroup("Settings")]
		public EAirVentBehavior Behavior;

		[FoldoutGroup("Settings")][Indent][ShowIf("IsManual")]
		public bool m_RebootAfterDelay;

		[FoldoutGroup("Settings")][Indent(2)][ShowIf("ShowRebootDelay")]
		public float rebootTime;

		[SerializeField][FoldoutGroup("Debug")]
		bool m_AirVentOn;

		[FoldoutGroup("Settings")][Indent][ShowIf("IsIntermitent")]
		public float turnedOnTime = 2;

		[FoldoutGroup("Settings")][Indent][ShowIf("IsIntermitent")]
		public float turnedOffTime = 2;

		float m_CurrentTime;

		bool IsIntermitent()
		{
			if (Behavior == EAirVentBehavior.Intermitent)
			{
				return true;
			}

			else return false;
		}

		bool IsManual()
		{
			if (Behavior == EAirVentBehavior.Manual)
			{
				return true;
			}

			else return false;
		}

		bool ShowRebootDelay()
		{
			if (Behavior == EAirVentBehavior.Manual && m_RebootAfterDelay)
			{
				return true;
			}

			else return false;
		}

		public void TurnOn()
		{
			areaEffector.enabled = true;
			particuleSystem.Play();
			m_AirVentOn = true;
		}

		public void TurnOff()
		{
			areaEffector.enabled = false;
			particuleSystem.Stop();
			m_AirVentOn = false;
		}

		// Start is called before the first frame update
		void Start()
		{
			if (startOn)
			{
				TurnOn();
			}

			else
			{
				TurnOff();
			}
		}

		// Update is called once per frame
		void Update()
		{
			if(Behavior == EAirVentBehavior.Intermitent)
			{
				m_CurrentTime += Time.deltaTime;

				if (m_AirVentOn)
				{
					if(m_CurrentTime >= turnedOnTime)
					{
						TurnOff();
						m_CurrentTime = 0;
					}
				}

				else
				{
					if (m_CurrentTime >= turnedOffTime)
					{
						TurnOn();
						m_CurrentTime = 0;
					}
				}
			}

			if(Behavior == EAirVentBehavior.Manual)
			{
				if (!m_AirVentOn && m_RebootAfterDelay)
				{
					m_CurrentTime += Time.deltaTime;
					if(m_CurrentTime >= rebootTime)
					{
						TurnOn();
						m_CurrentTime = 0;
					}
				}
			}
		}

		public void SwitchState()
		{
			if (m_AirVentOn)
			{
				TurnOff();
			}

			else
			{
				TurnOn();
			}
		}

		public enum EAirVentBehavior { Continuous, Manual, Intermitent }
	}
}
