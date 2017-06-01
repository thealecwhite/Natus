using System.Collections.Generic;

using UnityEngine;

public class Loot : MonoBehaviour, IInteractable
{
	public List<Item> items;
	public string interactInfo { get { return "Loot"; } }

	public bool CanInteract(PlayerMob mob)
	{
		return !mob.ignoreMoveInput && mob.isOnGround;
	}

	public void OnInteract(PlayerMob mob)
	{
		PlayerState.instance.itemStock.AddItem(items.ToArray());
		GameState.inGameMenu.SetItemGet(items.ToArray());

		// TESTING (REMOVE LATER)
		mob.EquipItem(GameState.items.sword, true);
		mob.EquipItem(GameState.items.shield, false);

		Destroy(gameObject);
	}

	public void OnInteractEnter()
	{
		
	}

	public void OnInteractExit()
	{
		
	}
}
