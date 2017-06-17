using System.Collections;

using UnityEngine;

public class SalesmanMob : AIMob<MobAnims, MobStates>, IInteractable
{
	public string interactInfo { get { return "Talk"; } }

	private int dialogueStage;

	protected override void Start()
	{
		base.Start();

		stateMachine.ChangeState(MobStates.Idle);
	}

	private void Idle_FixedUpdate()
	{
		if (isDead)
			return;

		RaycastHit2D playerHit = CheckForPlayerHit();

		if (playerHit)
		{
			target = playerHit.transform;
			transform.localScale = new Vector3(Mathf.Sign(target.position.x - transform.position.x), 1f, 1f);
		}

		MoveTo(home, 0.5f);
	}

	public override IEnumerator OnDamage(GameObject instigator, GameObject causer, float damage, float knockback)
	{
		OnInteractExit();

		GameState.inGameMenu.onDialogueContinue += OnDialogueContinue;
		OnDialogueContinue(3);

		return base.OnDamage(instigator, causer, damage, knockback);
	}

	protected override IEnumerator OnDeath()
	{
		OnInteractExit();

		GameState.inGameMenu.onDialogueContinue += OnDialogueContinue;
		OnDialogueContinue(4);

		return base.OnDeath();
	}

	public bool CanInteract(PlayerMob mob)
	{
		return !isDead && !mob.ignoreMoveInput && mob.isOnGround;
	}

	public void OnInteract(PlayerMob mob)
	{
		GameState.inGameMenu.onDialogueContinue += OnDialogueContinue;
		OnDialogueContinue(0);
	}

	public void OnInteractEnter()
	{

	}

	public void OnInteractExit()
	{
		GameState.inGameMenu.ClearDialogue();
	}

	private bool OnDialogueContinue()
	{
		string dialogue = string.Empty;

		switch (dialogueStage)
		{
			case 0:
				dialogue = "During my travels,\t=0.25f my most precious possession was stolen from me by an imp in the woods.";
				dialogueStage++;
				goto SetContinue;

			case 1:
				dialogue = "So here I am at a loss...";
				dialogueStage++;
				goto SetContinue;

			case 2:
				dialogue = "And now I've found you.";
				goto SetStop;

			case 3:
				dialogue = "What was that for?!";
				goto SetStop;

			case 4:
				dialogue = "Why\t=0.5f me\t=0.5f.\t=0.5f.\t=0.5f.";
				goto SetStop;

			SetContinue:
				GameState.inGameMenu.SetDialogue(dialogue, "Salesman");
				return true;

			SetStop:
				dialogueStage = -1;
				GameState.inGameMenu.SetDialogue(dialogue, "Salesman");
				return true;

			default:
				dialogueStage = 0;
				GameState.inGameMenu.ClearDialogue();
				return false;
		}
	}

	private bool OnDialogueContinue(int stage)
	{
		dialogueStage = stage;
		return OnDialogueContinue();
	}
}
