using UnityEngine;

public class Item : MonoBehaviour
{
	public new string name;
	[Multiline]
	public string description;

	/// <summary>
	/// How the item should function when "Use Item" button/key is pressed.
	/// </summary>
	/// <param name="mob">Which player is using the item.</param>
	public virtual void OnUse(PlayerMob mob)
	{
		mob.itemInUse = this;
	}

	public virtual bool CanUse(PlayerMob mob)
	{
		return !mob.ignoreMoveInput && mob.isOnGround;
	}

	public virtual void OnAnimEvent(PlayerMob mob, int i)
	{

	}
}
