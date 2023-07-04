using UnityEngine;

namespace AnimatorController
{
    public class DistractingMachineAnimatorController : AnimatorController
    {
        private static readonly int machineOn = Animator.StringToHash("On");

        public void PlayMachineOn()
        {
            animator.SetBool(machineOn, true);
        } 
        
        public void PlayMachineOff()
        {
            animator.SetBool(machineOn, false);
        }
    }
}