using System.Collections;
using Gameplay.InteractionSystem.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Controllers
{
	public abstract class Controller : MonoBehaviour, IController
	{
		[SerializeField][FoldoutGroup("Components")]
		protected Animator m_Animator;
		public Animator Animator => m_Animator;

		[SerializeField][FoldoutGroup("Components")]
		protected Rigidbody2D m_RB2D;
		public Rigidbody2D Rb2d => m_RB2D;

		[PropertyOrder(10)][SerializeField][FoldoutGroup("Debug")]
		protected float m_LookingDirection;
		
		public float LookingDirection => m_LookingDirection;

		protected IInteractable interactable;

		public void ChangeLookingDirection(float newLookingDirection)
		{
			m_LookingDirection = newLookingDirection;

			if (m_LookingDirection > 360)
			{
				m_LookingDirection -= 360;
			}

			if (m_LookingDirection < 0)
			{
				m_LookingDirection += 360;
			}
		}
	}
}