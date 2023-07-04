using System;
using Gameplay.EnergySystem.EnergyProduction;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.Controls.Controllers.AIControllers.Enemies.Units
{
    [Serializable]
    public abstract class UnitJob
    {
        [SerializeField]
        protected EJobType m_jobType;
        public EJobType JobType => m_jobType;
    
        public EJobCompleteCondition endJobOn;

        public bool jobCompleted;

        private bool JobIsControlledElement()
        {
            return m_jobType == EJobType.ModifyControlledElementState;
        }

        protected void JobCompleted()
        {
            jobCompleted = true;
        }
    
        public enum EJobType
        {
            PowerSocket,
            ModifyControlledElementState,
            CheckSocket,
            TurnOnDistractingMachine,
        }
    
        public enum EJobCompleteCondition
        {
            Event,
            JobCompleted
        }
    }

    public class PowerSocketJob : UnitJob
    {
        public EnergySocket Socket { get; }

        public PowerSocketJob(EnergySocket socket, UnityEvent onJobCompleted)
        {
            m_jobType = EJobType.PowerSocket;
            endJobOn = EJobCompleteCondition.Event;
            Socket = socket;
            onJobCompleted.AddListener(JobCompleted);
        }
    }

    public class TurnOnDistractingMachineJob : UnitJob
    {
        public DistractingMachine DistractingMachine { get; }
        
        public TurnOnDistractingMachineJob(DistractingMachine distractingMachine)
        {
            DistractingMachine = distractingMachine;
            m_jobType = EJobType.TurnOnDistractingMachine;
            endJobOn = EJobCompleteCondition.JobCompleted;
        }
    }

    public class CheckSocketJob : UnitJob
    {
        public EnergySource Socket { get; }

        public CheckSocketJob(EnergySource socket)
        {
            m_jobType = EJobType.CheckSocket;
            endJobOn = EJobCompleteCondition.JobCompleted;
            Socket = socket;
        }
    }

    public class MonitorControlledElementJob : UnitJob
    {
        public MovingPoweredSystem MovingPoweredSystem { get; }

        public EControlledElementState DesiredState { get; }

        public MonitorControlledElementJob(MovingPoweredSystem movingPoweredSystem, EControlledElementState desiredState, UnityEvent onJobFinished)
        {
            m_jobType = EJobType.ModifyControlledElementState;
            endJobOn = EJobCompleteCondition.Event;
            MovingPoweredSystem = movingPoweredSystem;
            DesiredState = desiredState;
            onJobFinished.AddListener(JobCompleted);
        } 
        public MonitorControlledElementJob(MovingPoweredSystem movingPoweredSystem, EControlledElementState desiredState)
        {
            m_jobType = EJobType.ModifyControlledElementState;
            endJobOn = EJobCompleteCondition.JobCompleted;
            MovingPoweredSystem = movingPoweredSystem;
            DesiredState = desiredState;
        }
    }
}