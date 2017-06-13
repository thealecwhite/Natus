using System.Collections;

using UnityEngine;

public class SalesmanMob : AIMob<MobAnims, MobStates>, IInteractable
{
	public string interactInfo { get { return "Talk"; } }

	private int dialogueStage;

	public override IEnumerator OnDamage(GameObject instigator, GameObject causer, float damage, float knockback)
	{
		OnInteractExit();

		return base.OnDamage(instigator, causer, damage, knockback);
	}

	public bool CanInteract(PlayerMob mob)
	{
		return !isDead && !mob.ignoreMoveInput && mob.isOnGround;
	}

	public void OnInteract(PlayerMob mob)
	{
		GameState.inGameMenu.onDialogueContinue += OnDialogueContinue;
		dialogueStage = 0;
		OnDialogueContinue();
	}

	public void OnInteractEnter()
	{

	}

	public void OnInteractExit()
	{
		dialogueStage = 0;
		GameState.inGameMenu.ClearDialogue();
	}

	private bool OnDialogueContinue()
	{
		string dialogue = string.Empty;

		switch (dialogueStage)
		{
			case 0:
				dialogue = "During my travels,\t=0.25f my most precious possession was stolen from me by an imp in the woods.";
				goto dialogue;

			case 1:
				dialogue = "So here I am at a loss...";
				goto dialogue;

			case 2:
				dialogue = "And now I've found you.";
				goto dialogue;

			dialogue:
				GameState.inGameMenu.SetDialogue(dialogue, "Salesman");
				dialogueStage++;
				return true;

			default:
				dialogueStage = 0;
				GameState.inGameMenu.ClearDialogue();
				return false;
		}
	}
}
