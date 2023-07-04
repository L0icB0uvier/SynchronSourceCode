using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.SecuritySystem;
using UnityEngine;
using Utilities;


[TaskCategory("Movement/Rotation")]
public class RotateSpy : Action
{
	public SharedAIController AIController;

	WatchTowerAIController m_watchTowerAIController;

	int currentAngleIndex;
	bool increaseIndex = true;

	bool m_Wait;
	float currentWaitTime;
	private float waitTime;

	float m_NewAngle;

	float m_RotationSpeed;

	float velocity;

	public float smoothTime = .5f;

	public override void OnStart()
	{
		m_watchTowerAIController = (WatchTowerAIController)AIController.Value;

		switch (m_watchTowerAIController.m_RotationType)
		{
			case ESpyBehavior.RotateCirle:
				if (m_watchTowerAIController.ClockwiseRotation)
				{
					m_RotationSpeed = -m_watchTowerAIController.rotationSpeed;
				}

				else
				{
					m_RotationSpeed = m_watchTowerAIController.rotationSpeed;
				}

				break;
			case ESpyBehavior.RotateBetweenAngles:

				m_Wait = false;
				currentWaitTime = 0;
				currentAngleIndex = 1;
				break;
		}	
	}

	public override void OnConditionalAbort()
	{
		base.OnConditionalAbort();
		m_Wait = false;
		currentWaitTime = 0;
	}

	public override TaskStatus OnUpdate()
	{
		switch (m_watchTowerAIController.m_RotationType)
		{
			case ESpyBehavior.Fixe:
				if (Mathf.Abs(Mathf.DeltaAngle(AIController.Value.LookingDirection, m_watchTowerAIController.fixeAngle)) > 1)
				{
					m_NewAngle = Mathf.MoveTowardsAngle(AIController.Value.LookingDirection, m_watchTowerAIController.fixeAngle, 90 * Time.deltaTime);
					AIController.Value.ChangeLookingDirection(m_NewAngle);
				}
				return TaskStatus.Running;

			case ESpyBehavior.RotateCirle:

				m_NewAngle = AIController.Value.LookingDirection + m_RotationSpeed * Time.deltaTime;
				AIController.Value.ChangeLookingDirection(m_NewAngle);

				return TaskStatus.Running;

			case ESpyBehavior.RotateBetweenAngles:

				if (m_Wait)
				{
					currentWaitTime += Time.deltaTime;
					if (currentWaitTime >= waitTime)
					{
						m_Wait = false;
					}

					else
					{
						return TaskStatus.Running;
					}
				}

				//TargetAngleReached
				if (Mathf.Abs(Mathf.DeltaAngle(AIController.Value.LookingDirection, m_watchTowerAIController.angles[currentAngleIndex].lookingAngle)) < 2)
				{
					if (m_watchTowerAIController.angles[currentAngleIndex].waitingTime > 0)
					{
						m_Wait = true;
						waitTime = m_watchTowerAIController.angles[currentAngleIndex].waitingTime;
						currentWaitTime = 0;
					}
				
					switch (m_watchTowerAIController.RotationCompleteBehavior)
					{
						case EAngleRotationCompletedBehavior.Loop:
							if (currentAngleIndex == m_watchTowerAIController.angles.Length - 1)
							{
								currentAngleIndex = 0;
							}

							else
							{
								currentAngleIndex++;
							}
							break;
						case EAngleRotationCompletedBehavior.PingPong:
							if (increaseIndex && currentAngleIndex == m_watchTowerAIController.angles.Length - 1)
							{
								increaseIndex = false;
							}

							else if(!increaseIndex && currentAngleIndex == 0)
							{
								increaseIndex = true;
							}

							if (increaseIndex)
							{
								currentAngleIndex++;
							}
							
							else
							{
								currentAngleIndex--;
							}
						
							break;
					}
				}

				m_NewAngle = Mathf.SmoothDampAngle(AIController.Value.LookingDirection, m_watchTowerAIController.angles[currentAngleIndex].lookingAngle, ref velocity, smoothTime, m_watchTowerAIController.angles[currentAngleIndex].rotationSpeed, Time.deltaTime);
				if (MathCalculation.ApproximatelyEqualFloat(m_NewAngle, 360, 1))
					m_NewAngle = 0;

				AIController.Value.ChangeLookingDirection(m_NewAngle);

				return TaskStatus.Running;

			default:
				return TaskStatus.Running;
		}

	
	}
}
