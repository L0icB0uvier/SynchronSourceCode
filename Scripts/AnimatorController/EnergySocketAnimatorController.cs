using UnityEngine;

namespace AnimatorController
{
    public class EnergySocketAnimatorController : TransitioningSystemAnimatorController
    {
        [SerializeField] private string unpowered_Closed = "Unpowered_Closed";
        [SerializeField] private string unpowered_Opening = "Unpowered_Opening";
        [SerializeField] private string unpowered_Closing = "Unpowered_Closing" ;
        [SerializeField] private string cellPowered_Closed = "CellPowered_Closed";
        [SerializeField] private string cellPowered_Opening = "CellPowered_Opening";
        [SerializeField] private string cellPowered_Closing = "CellPowered_Closing";

        private static readonly int energyCellPlugged = Animator.StringToHash("EnergyCellPlugged");
        private static readonly int poweredByRobot = Animator.StringToHash("PoweredByRobot");

        private bool m_cellPlugged;
       
        public void SetEnergyCellPluggedParameter(bool plugged)
        {
            m_cellPlugged = plugged;
            animator.SetBool(energyCellPlugged, plugged);
        }

        public void SetPoweredByRobotParameter(bool powered)
        {
            animator.SetBool(poweredByRobot, powered);
        }
        
        public void PlayTransitionToOpened()
        {
            if (!CanTransitionToAltered()) return;

            var currentState = animator.GetCurrentAnimatorStateInfo(0);

            animator.PlayInFixedTime(m_cellPlugged ? cellPowered_Opening : unpowered_Opening, 0, currentState.IsTag("TransitionToAltered") ? 1 - currentState.normalizedTime : 0);
        }
        
        public void PlayTransitionToClosed()
        {
            if (!CanTransitionToDefault()) return;
            
            var currentState = animator.GetCurrentAnimatorStateInfo(0);

            animator.PlayInFixedTime(m_cellPlugged ? cellPowered_Closing : unpowered_Closing, 0, currentState.IsTag("TransitionToAltered") ? 1 - currentState.normalizedTime : 0);
        }

        public void PlaySocketClosed()
        {
            animator.Play(m_cellPlugged? cellPowered_Closed : unpowered_Closed);
        }
    }
}