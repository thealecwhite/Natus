using UnityEngine;

public enum WieldType
{
	MainHand, OffHand, TwoHand
}

public class WeaponItem : Item
{
	public WieldType wieldType;
	public int damage;

	private void Start()
	{
		BoxCollider2D collider = GetComponent<BoxCollider2D>();

		if (collider)
			collider.enabled = false;
	}
}
