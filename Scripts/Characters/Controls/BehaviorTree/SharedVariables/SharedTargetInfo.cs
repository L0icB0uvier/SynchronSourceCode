using BehaviorDesigner.Runtime;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.SharedVariables
{
    [System.Serializable]
    public class SharedTargetInfo : SharedVariable<TargetInfo>
    {
        public static implicit operator SharedTargetInfo(TargetInfo value)
        { return new SharedTargetInfo { Value = value }; }
    }
}