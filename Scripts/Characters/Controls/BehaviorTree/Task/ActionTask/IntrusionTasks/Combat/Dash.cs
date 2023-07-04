using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Combat
{
	[TaskCategory("Behavior/Combat")]
	public class Dash : Action
	{
		//public SharedAIController AIController;

		//SentryAIController m_SentryAIController;

		//DashAttack m_DashAttack;

		//public SharedGameObject Intruder;

		//SeeTarget see;

		//bool m_IsDashing = false;


		//public override void OnAwake()
		//{
		//	base.OnAwake();

		//	m_SentryAIController = (SentryAIController)AIController.Value;
		//	m_DashAttack = m_SentryAIController.Dash;
		//	see = Owner.FindTask<SeeTarget>();
		//}

		//public override void OnStart()
		//{
		//	Vector2 m_DashDirection = (Intruder.Value.transform.position - AIController.Value.transform.position).normalized;
		//	Vector2 dashFinalPos = GetDashFinalPosition(m_DashDirection);
		//	m_DashAttack.StartDashing(m_DashDirection, dashFinalPos, OnDashCompleted);
		//	m_IsDashing = true;
		//	see.canSee = false;
		
		//}

		//private Vector2 GetDashFinalPosition(Vector2 dashDirection)
		//{
		//	Vector2 intruderPos = Intruder.Value.transform.position;
		
		//	for (int i = m_DashAttack.dashEndPosOffset; i > 0; i--)
		//	{
		//		Vector2 dashEndPos = intruderPos + dashDirection * i;

		//		if (Physics2D.OverlapCircle(dashEndPos, 1, m_DashAttack.groundLayerMask) && !Physics2D.OverlapCircle(dashEndPos, 2, m_DashAttack.dashBlockingLayerMask))
		//		{
		//			return dashEndPos;
		//		}
		//	}

		//	return intruderPos;
		//}

		//public override TaskStatus OnUpdate()
		//{
		//	if (m_IsDashing)
		//	{
		//		return TaskStatus.Running;
		//	}

		//	else
		//	{
		//		AIController.Value.ChangeLookingDirection(MathCalculation.ConvertDirectionToAngle((Intruder.Value.transform.position - AIController.Value.transform.position).normalized));
		//		see.canSee = true;
		//		return TaskStatus.Success;
		//	}		
		//}

		//public void OnDashCompleted()
		//{
		//	m_IsDashing = false;
		//}
	}
}