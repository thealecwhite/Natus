using System.Collections.Generic;

using UnityEngine;

using UnityToolbag;

public class Loot : MonoBehaviour, IInteractable
{
	[SortableArray]
	public List<Item> items;
	public string interactInfo { get { return "Loot"; } }

	public bool CanInteract(PlayerMob mob)
	{
		return !mob.ignoreMoveInput && mob.isOnGround;
	}

	public void OnInteract(PlayerMob mob)
	{
		PlayerState.instance.inventoryStock.AddItem(items.ToArray());
		GameState.inGameMenu.SetItemGet(items.ToArray());

		GameState.player.EquipItem(items[0]);
		GameState.player.EquipItem(GameState.items.shield);

		Destroy(gameObject);
	}

	public void OnInteractEnter()
	{
		
	}

	public void OnInteractExit()
	{
		
	}
}
