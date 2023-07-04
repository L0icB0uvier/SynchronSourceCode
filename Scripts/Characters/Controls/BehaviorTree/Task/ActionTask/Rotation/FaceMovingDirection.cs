using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Rotation
{
	[TaskCategory("Movement/Rotation")]
	public class FaceMovingDirection : Action
	{
		public SharedAIController AIController;

		Rigidbody2D m_RB2D;

		public override void OnAwake()
		{
			base.OnAwake();

			m_RB2D = transform.GetComponentInParent<Rigidbody2D>();
		}

		public override TaskStatus OnUpdate()
		{
			if(m_RB2D.velocity.magnitude > .1f)
				AIController.Value.ChangeLookingDirection(MathCalculation.ConvertDirectionToAngle(m_RB2D.velocity.normalized));

			return TaskStatus.Running;
		}
	}
}