using UnityEngine;

public class PlayerState : Singleton<PlayerState>
{
	public ItemStock inventoryStock = new ItemStock(false), storageStock = new ItemStock(true);
	public Item shortcutItem1, shortcutItem2, shortcutItem3, shortcutItem4;
}
