using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Investigate
{
   [TaskCategory("Behavior/Investigate")]
   [TaskDescription("Go investigate a location and look around")]
   public class UnitInvestigate : Action
   {
      public SharedVector2 InvestigateLocation;
   
      public override void OnAwake()
      {
         base.OnAwake();
      }

      public override void OnStart()
      {
         base.OnStart();
      }

      public override TaskStatus OnUpdate()
      {
         return base.OnUpdate();
      
      }
   }
}
