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

	/// <summary>
	/// How the item should function when "Use Item" button/key is released.
	/// </summary>
	/// <param name="mob">Which player is using the item.</param>
	public virtual void OnEndUse(PlayerMob mob)
	{

	}
}
