using UnityEngine;

public class WeaponItem : Item
{
	public int damage;

	private void Start()
	{
		BoxCollider2D collider = GetComponent<BoxCollider2D>();

		if (collider)
			collider.enabled = false;
	}
}
