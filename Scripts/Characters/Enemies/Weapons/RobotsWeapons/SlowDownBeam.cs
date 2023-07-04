using UnityEngine;

namespace Characters.Ennemies.Weapons.RobotsWeapons
{
	public class SlowDownBeam : Attack
	{
		[Tooltip("Speed debuf strenght to apply, 1 meaning no debuf and 0 meaning no more movenment for the target")]
		public float debufStrenght = 1;

		//public override void TargetEnterWeaponRange()
		//{
		//	base.TargetEnterWeaponRange();

		//	AIController.Target.GetComponent<Controller>().ApplySpeedDebuf(debufStrenght);
		//}

		//public override void TargetExitWeaponRange()
		//{
		//	base.TargetExitWeaponRange();

		//	AIController.Target.GetComponent<Controller>().RemoveSpeedDebuf();
		//}
	}
}
