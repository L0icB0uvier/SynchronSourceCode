using Characters.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Characters.Ennemies.Weapons.Soldier
{
	public class SoldierBullet : Projectile
	{
		SpriteRenderer m_BulletRenderer;

		protected TrailRenderer m_TrailRenderer;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet0;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet30;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet60;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet90;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet120;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet150;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet180;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet210;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet240;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet270;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet300;

		[FoldoutGroup("Bullet Sprites")]
		public Sprite bullet330;

		protected override void Awake()
		{
			base.Awake();
			m_BulletRenderer = GetComponent<SpriteRenderer>();
			m_TrailRenderer = GetComponent<TrailRenderer>();
		}

		public override void ShootProjectile(Vector2 shotFrom, Vector2 force, GameObject shooter)
		{
			base.ShootProjectile(shotFrom, force, shooter);

			SetBulletSprite(MathCalculation.ConvertDirectionToAngle(force.normalized));
			m_TrailRenderer.emitting = true;
		}

		public override void OnDespawn()
		{
			base.OnDespawn();
			m_TrailRenderer.Clear();
		}

		void SetBulletSprite(float movementDirection)
		{
			if (movementDirection > 0 && movementDirection <= 15f)
				m_BulletRenderer.sprite = bullet0;
			else if (movementDirection > 15 && movementDirection <= 45)
				m_BulletRenderer.sprite = bullet30;
			else if (movementDirection > 45 && movementDirection <= 75)
				m_BulletRenderer.sprite = bullet60;
			else if (movementDirection > 75 && movementDirection <= 105)
				m_BulletRenderer.sprite = bullet90;
			else if (movementDirection > 105 && movementDirection <= 135)
				m_BulletRenderer.sprite = bullet120;
			else if (movementDirection > 135 && movementDirection <= 165)
				m_BulletRenderer.sprite = bullet150;
			else if (movementDirection > 165 && movementDirection <= 195)
				m_BulletRenderer.sprite = bullet180;
			else if (movementDirection > 195 && movementDirection <= 225)
				m_BulletRenderer.sprite = bullet210;
			else if (movementDirection > 225 && movementDirection <= 255)
				m_BulletRenderer.sprite = bullet240;
			else if (movementDirection > 255 && movementDirection <= 285)
				m_BulletRenderer.sprite = bullet270;
			else if (movementDirection > 285 && movementDirection <= 315)
				m_BulletRenderer.sprite = bullet300;
			else if (movementDirection > 315 && movementDirection <= 345)
				m_BulletRenderer.sprite = bullet330;
			else if (movementDirection > 345 && movementDirection <= 360)
				m_BulletRenderer.sprite = bullet0;
		}
	}
}
