using UnityEngine;

public class ShieldWeaponItem : WeaponItem
{
	public override void OnUse(PlayerMob mob)
	{
		mob.ignoreMoveInput = true;
		mob.hasShieldUp = true;
		mob.animator.Play(mob.anims.defend);
	}

	public override void OnEndUse(PlayerMob mob)
	{
		if (mob.hasShieldUp)
		{
			mob.ignoreMoveInput = false;
			mob.hasShieldUp = false;
		}
	}
}
