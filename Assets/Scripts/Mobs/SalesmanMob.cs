using System.Collections;

using UnityEngine;

public class SalesmanMob : AIMob<MobAnims, MobStates>, IInteractable
{
	public string interactInfo { get { return "Talk"; } }

	private int dialogueStage;

	protected override IEnumerator AI()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();

			RaycastHit2D hit = CheckForPlayerHit();

			if (hit)
				transform.localScale = new Vector3(Mathf.Sign((hit.transform.position - transform.position).x), 1f, 1f);
		}
	}

	public override IEnumerator OnDamage(float damage, GameObject instigator, GameObject causer)
	{
		OnInteractExit();

		return base.OnDamage(damage, instigator, causer);
	}

	public void OnInteract(PlayerMob mob)
	{
		GameState.inGameMenu.onDialogueContinue += OnDialogueContinue;
		dialogueStage = 0;
		OnDialogueContinue();
	}

	public bool CanInteract(PlayerMob mob)
	{
		return !mob.ignoreMoveInput && mob.isOnGround;
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
