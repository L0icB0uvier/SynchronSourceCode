using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.CombatCheck
{
	[TaskCategory("Combat")]
	public class CanUseDistanceAttack : Conditional
	{
		//public SharedAIController AIController;
		//SentryAIController m_SentryAICOntroller;

		//SentryRangeAttack m_RangeAttack;

		//public SharedGameObject intruder;

		//public int MinRange = 15;
		//public int MaxRange = 20;

		//public override void OnAwake()
		//{
		//	base.OnAwake();

		//	m_SentryAICOntroller = (SentryAIController)AIController.Value;
		//	m_RangeAttack = m_SentryAICOntroller.RangeAttack;
		//}

		//public override TaskStatus OnUpdate()
		//{
		//	if (!m_RangeAttack.CanBeUsed)
		//		return TaskStatus.Failure;

		//	float sqrRange = (intruder.Value.transform.position - AIController.Value.transform.position).sqrMagnitude;
		//	if (sqrRange > m_RangeAttack.minRange * m_RangeAttack.minRange && sqrRange < m_RangeAttack.maxRange * m_RangeAttack.maxRange)
		//	{
		//		return TaskStatus.Success;
		//	}

		//	else return TaskStatus.Failure;
		//}
	}
}