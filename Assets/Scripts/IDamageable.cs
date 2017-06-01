using System.Collections;

using UnityEngine;

public interface IDamageable
{
	bool isDamageable { get; }

	IEnumerator OnDamage(float damage, GameObject instigator, GameObject causer);
}
