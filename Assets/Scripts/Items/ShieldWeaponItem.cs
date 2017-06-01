using UnityEngine;

public class ShieldWeaponItem : WeaponItem
{
	public override void OnUse(PlayerMob mob)
	{
		base.OnUse(mob);

		mob.animator.Play(mob.anims.deflect);
		mob.ignoreMoveInput = true;
	}
}
