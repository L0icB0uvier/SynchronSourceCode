using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Enemies.Perception;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.PerceptionCheck
{
	[TaskCategory("Perception")]
	public class SeeTarget : Conditional
	{
		public SharedAIController AIController;
		FieldOfView fov;

		public SharedGameObject target;

		public bool canSee = true;

		public override void OnAwake()
		{
			base.OnAwake();

			fov = AIController.Value.GetComponentInChildren<FieldOfView>();
		}

		public override TaskStatus OnUpdate()
		{
			if (!canSee)
			{
				return TaskStatus.Success;
			}

			if (!fov)
			{
				fov = AIController.Value.GetComponentInChildren<FieldOfView>();
			}

			if (fov.TargetsAcquired.Count > 0)
			{

				target.Value = fov.TargetsAcquired[0];
				return TaskStatus.Success;
			}

			else
			{
				return TaskStatus.Failure;
			}
		}

		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			base.OnBehaviorComplete();
			canSee = true;
		}
	}
}