using UnityEngine;

public class Item : MonoBehaviour
{
	public new string name;
	[Multiline]
	public string description;
	public int stockLimit = 99;

	public virtual bool CanUse(PlayerMob mob)
	{
		return !mob.ignoreMoveInput && mob.isOnGround;
	}

	/// <summary>
	/// How the item should function when "Use Item" button/key is pressed.
	/// </summary>
	/// <param name="mob">Which player is using the item.</param>
	public virtual void OnUse(PlayerMob mob)
	{

	}

	/// <summary>
	/// How the item should function when "Use Item" button/key is released.
	/// </summary>
	/// <param name="mob">Which player is using the item.</param>
	public virtual void OnEndUse(PlayerMob mob)
	{

	}

	/// <summary>
	/// How the item should function when "Use Item" button/key is being held.
	/// </summary>
	/// <param name="mob">Which player is using the item.</param>
	public virtual void OnHoldUse(PlayerMob mob)
	{

	}

	public virtual void OnAnimEvent(PlayerMob mob, int i)
	{

	}
}
