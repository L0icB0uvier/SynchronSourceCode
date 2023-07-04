using SceneManagement.LevelManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Controllers.AIControllers.Enemies
{
	public abstract class EnemyAIController : AIController
	{
		[SerializeField][FoldoutGroup("Components")] protected BehaviorDesigner.Runtime.BehaviorTree enemyBehaviorTree;
	
		[SerializeField][FoldoutGroup("Debug")]
		private AreaManager m_CurrentArea;
		public AreaManager CurrentArea => m_CurrentArea;

		[FoldoutGroup("AI")]
		public EEnemyDefaultBehavior defaultBehavior;

		protected virtual void Awake()
		{
			m_CurrentArea = FindObjectOfType<AreaManager>();
		}

		protected override void Start()
		{
			base.Start();
			EnableBehaviorTree(enemyBehaviorTree);
		}

		protected override void InitializeBtValues()
		{
			enemyBehaviorTree.SetVariableValue("AIController", this);
		}
	}
	
	public enum EEnemyDefaultBehavior { Default, Rest }
}