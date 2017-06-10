using System.Collections;

using UnityEngine;

public interface IDamageable
{
	bool isDamageable { get; }

	IEnumerator OnDamage(GameObject instigator, GameObject causer, float damage, float knockback);
}
