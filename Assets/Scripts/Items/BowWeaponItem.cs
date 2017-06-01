using UnityEngine;

public class BowWeaponItem : WeaponItem
{
	public Sprite regularBow, drawnBow;

	public override void OnUse(PlayerMob mob)
	{
		base.OnUse(mob);

		mob.animator.Play(mob.animations.drawBow);
		mob.ignoreMoveInput = true;
	}

	public override void OnEndUse(PlayerMob mob)
	{
		base.OnEndUse(mob);

		mob.animator.Play(mob.animations.releaseBow);
	}
}
