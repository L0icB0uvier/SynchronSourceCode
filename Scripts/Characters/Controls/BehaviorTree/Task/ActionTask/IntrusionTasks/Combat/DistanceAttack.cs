using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Combat
{
	[TaskCategory("Behavior/Combat")]
	public class DistanceAttack : Action
	{
		//public SharedAIController AIController;
		//SentryAIController m_SentryAIController;

		//public SharedGameObject intruder;

		//SentryRangeAttack m_RangeAttack;

		//bool attackInProgress = false;

		//SeeTarget see;

		//public override void OnAwake()
		//{
		//	base.OnAwake();
		//	m_SentryAIController = (SentryAIController)AIController.Value;
		//	m_RangeAttack = m_SentryAIController.RangeAttack;
		//	see = Owner.FindTask<SeeTarget>();
		//}

		//public override void OnStart()
		//{
		//	base.OnStart();
		//	m_RangeAttack.LaunchAttack(FindAttackLocation(), OnAttackCompleted);
		//	attackInProgress = true;
		//	see.canSee = false;
		//}

		//public override TaskStatus OnUpdate()
		//{
		//	if (attackInProgress)
		//	{
		//		return TaskStatus.Running;
		//	}

		//	else
		//	{
		//		see.canSee = true;
		//		return TaskStatus.Success;
		//	}
		//}

		//Vector2 FindAttackLocation()
		//{
		//	Vector2 intruderDirection = intruder.Value.GetComponent<Rigidbody2D>().velocity.normalized;
		//	Vector2 intruderPos2D = intruder.Value.transform.position;
		//	Vector2 rangeAttackLocation = intruderPos2D + intruderDirection * m_RangeAttack.attackOffset;

		//	for (int i = m_RangeAttack.attackOffset; i > 0; i--)
		//	{
		//		Vector2 correctedLocation = intruderPos2D + intruderDirection * i;
		//		if (Physics2D.OverlapCircle(correctedLocation, 1, m_RangeAttack.groundLayerMask) && !Physics2D.OverlapCircle(correctedLocation, 2, m_RangeAttack.obstaclesLayerMask))
		//		{
		//			return correctedLocation;
		//		}
		//	}

		//	return intruderPos2D;	
		//}

		//public void OnAttackCompleted()
		//{
		//	attackInProgress = false;
		//	see.canSee = true;
		//}
	}
}