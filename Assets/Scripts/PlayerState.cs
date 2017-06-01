using UnityEngine;

public class PlayerState : Singleton<PlayerState>
{
	protected PlayerState() : base() {}

	public ItemStock itemStock = new ItemStock(999);
}
