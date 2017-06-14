using UnityEngine;

public class ShieldWeaponItem : WeaponItem
{
	public override bool CanUse(PlayerMob mob)
	{
		return (mob.ignoreMoveInput == mob.hasShieldUp) && mob.isOnGround;
	}

	public override void OnEndUse(PlayerMob mob)
	{
		if (mob.hasShieldUp)
		{
			mob.ignoreMoveInput = false;
			mob.hasShieldUp = false;
		}
	}

	public override void OnHoldUse(PlayerMob mob)
	{
		if (CanUse(mob))
		{
			if (!mob.hasShieldUp)
			{
				mob.ignoreMoveInput = true;
				mob.hasShieldUp = true;
				mob.animator.Play(mob.anims.defend);
			}
		}
		else OnEndUse(mob);
	}
}
