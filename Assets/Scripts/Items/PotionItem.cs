using UnityEngine;

public class PotionItem : Item
{
	public override void OnUse(PlayerMob mob)
	{
		base.OnUse(mob);

		mob.ignoreMoveInput = true;
		mob.animator.Play(mob.anims.usePotion);
	}

	public override void OnAnimEvent(PlayerMob mob, int i)
	{
		mob.Heal(mob.maxHP * 0.3f);
		PlayerState.instance.itemStock.RemoveItem(this);
	}
}
