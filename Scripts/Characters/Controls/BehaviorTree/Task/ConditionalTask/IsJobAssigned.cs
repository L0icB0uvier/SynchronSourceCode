using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Pathfinding;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	[TaskCategory("Job")]
	[TaskDescription("Check if the unit has a job assigned")]
	public class IsJobAssigned : Conditional
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		public SharedVector2 JobLocation;
		public SharedVector2 JobInteractionLocation;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			if (m_unitAIController.JobsAssigned.Count == 0)
				return TaskStatus.Failure;
		
			switch (m_unitAIController.JobsAssigned[0].JobType)
			{
				case UnitJob.EJobType.CheckSocket:
					var checkSocketJob = (CheckSocketJob)m_unitAIController.JobsAssigned[0];
					var socketToCheckLocation = checkSocketJob.Socket.transform.position;
					JobLocation.Value = socketToCheckLocation;
					JobInteractionLocation.Value = socketToCheckLocation;
					break;
			
				case UnitJob.EJobType.PowerSocket:
					var powerJob = (PowerSocketJob)m_unitAIController.JobsAssigned[0];
					var socketLocation = powerJob.Socket.transform.position;
					JobLocation.Value = socketLocation;
					JobInteractionLocation.Value = socketLocation;
					break;
				case UnitJob.EJobType.ModifyControlledElementState:
					var MonitorElementJob = (MonitorControlledElementJob)m_unitAIController.JobsAssigned[0];
					var s = PathfindingUtilities.FindAccessibleInteractable(MonitorElementJob.MovingPoweredSystem, transform
						.position, GraphMask.FromGraphName("AreaGraph"));
					var position = s.transform.position;
					JobLocation.Value = position;
					var dir = MathCalculation.GetDirectionalVectorBetween2Points(m_unitAIController.transform.position,
						position);

					JobInteractionLocation.Value = (Vector2)position - dir * 2;
					break;
				case UnitJob.EJobType.TurnOnDistractingMachine:
					var turnOnJob = (TurnOnDistractingMachineJob)m_unitAIController.JobsAssigned[0];
					JobLocation.Value = turnOnJob.DistractingMachine.Interactable.transform.position;
					JobInteractionLocation.Value = turnOnJob.DistractingMachine.InteractionLocation.position ;
					break;
			}

			return TaskStatus.Success;
		}
	}
}