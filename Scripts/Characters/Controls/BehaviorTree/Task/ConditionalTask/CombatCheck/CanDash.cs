using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.CombatCheck
{
	[TaskCategory("Combat")]
	public class CanDash : Conditional
	{
		//public SharedAIController AIController;

		//SentryAIController m_SentryAIController;

		//DashAttack m_Dash;

		//public SharedGameObject intruder;

		//float colliderRadius;

		//public override void OnAwake()
		//{
		//	base.OnAwake();

		//	m_SentryAIController = (SentryAIController)AIController.Value;
		//	m_Dash = m_SentryAIController.Dash;

		//	colliderRadius = m_SentryAIController.transform.root.GetComponent<CircleCollider2D>().radius;
		//}

		//public override TaskStatus OnUpdate()
		//{
		//	if (m_Dash.CanBeUsed)
		//	{
		//		float sqrRange = (intruder.Value.transform.position - m_SentryAIController.transform.position).sqrMagnitude;
		//		if (sqrRange > m_Dash.minRange * m_Dash.minRange && sqrRange < m_Dash.maxRange * m_Dash.maxRange)
		//		{
		//			Vector2 targetDirection = (intruder.Value.transform.position - m_SentryAIController.transform.position).normalized;
		//			Vector2 pos2D = m_SentryAIController.transform.position;

		//			//Check for obstacles between target location and sentry
		//			if (!Physics2D.CircleCast(pos2D + targetDirection * (m_Dash.castRadius + colliderRadius + .25f), m_Dash.castRadius, targetDirection, Vector2.Distance(intruder.Value.transform.position, m_SentryAIController.transform.position), m_Dash.dashBlockingLayerMask.value))
		//			{
		//				return TaskStatus.Success;
		//			}

		//			else
		//			{
		//				return TaskStatus.Failure;
		//			}
		//		}

		//		else
		//			return TaskStatus.Failure;
		//	}

		//	else
		//	{
		//		return TaskStatus.Failure;
		//	}	
		//}

	}
}