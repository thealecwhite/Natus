using UnityEngine;

public class MeleeWeaponItem : WeaponItem
{
	public float speed = 1f;

	public override void OnUse(PlayerMob mob)
	{
		int temp = Random.Range(0, 2);

		print(temp);

		mob.animator.Play(temp == 0 ? mob.anims.swingDown : mob.anims.swingUp, speed);
		mob.ignoreMoveInput = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.isTrigger)
			return;

		IDamageable damageable = collision.GetComponent<IDamageable>();

		if (damageable != null)
			((MonoBehaviour)damageable).StartCoroutine(damageable.OnDamage(damage, transform.parent.gameObject, gameObject));
	}
}
