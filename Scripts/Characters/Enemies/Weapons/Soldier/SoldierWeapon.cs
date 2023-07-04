using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Ennemies.Weapons.Soldier
{
	public class SoldierWeapon : Attack
	{
		[FoldoutGroup("Weapon Settings")]
		public float dispersion = .5f;

		[FoldoutGroup("Weapon Settings")]
		public int shootStrenght = 30;

		[FoldoutGroup("Prefab Ref")]
		public GameObject bulletPrefab;

		public void FireWeapon(Transform target)
		{
			Vector2 targetDir = (target.position - transform.position).normalized;
			Vector2 location2D = transform.position;
			Vector2 shootFrom = location2D + Vector2.Perpendicular(targetDir) * Random.Range(-dispersion, dispersion);
			SoldierBullet bullet = LeanPool.Spawn(bulletPrefab, shootFrom, Quaternion.identity).GetComponent<SoldierBullet>();
			bullet.ShootProjectile(shootFrom, targetDir * shootStrenght, transform.root.gameObject);
			StartWeaponCooldown();
		}
	}
}
