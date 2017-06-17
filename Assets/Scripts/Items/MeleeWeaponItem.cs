using UnityEngine;

public class MeleeWeaponItem : WeaponItem
{
	public int damage;
	[Range(0.5f, 2f)]
	public float speed = 1f;
	[Range(0f, 10f)]
	public float range = 1f;
	[Range(0f, 5f)]
	public float knockback = 1f;

	public override void OnUse(PlayerMob mob)
	{
		mob.ignoreMoveInput = true;
		mob.PrepareMeleeDamage(this);
		mob.animator.Play(Random.Range(0, 2) == 0 ? mob.anims.swingDown : mob.anims.swingUp, speed);
	}

	public void DoDamage(Collider2D[] hits)
	{
		for (int i = 0; i < hits.Length; i++)
		{
			if (!hits[i])
				continue;

			IDamageable damageable = hits[i].GetComponent<IDamageable>();

			if (damageable != null)
			{
				((MonoBehaviour)damageable).StartCoroutine(damageable.OnDamage(transform.parent.gameObject, gameObject, damage, knockback));
				print(transform.parent.name + " hit " + hits[i].name);
			}
		}
	}
}
