using UnityEngine;

public class MeleeWeaponItem : WeaponItem
{
	public int damage;
	public float speed = 1f;
	public float range = 1f;

	public override void OnUse(PlayerMob mob)
	{
		mob.ignoreMoveInput = true;
		mob.PrepareMeleeDamage(this);
		mob.animator.Play(Random.Range(0, 2) == 0 ? mob.anims.swingDown : mob.anims.swingUp, speed);
	}

	public void DoMeleeDamage(Collider2D collision)
	{
		if (collision.isTrigger)
			return;

		IDamageable damageable = collision.GetComponent<IDamageable>();

		if (damageable != null)
			((MonoBehaviour)damageable).StartCoroutine(damageable.OnDamage(damage, transform.parent.gameObject, gameObject));
	}
}
