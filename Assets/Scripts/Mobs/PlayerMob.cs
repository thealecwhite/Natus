using System.Collections;

using UnityEngine;

using PowerTools;

[System.Serializable]
public class PlayerMobAnims : MobAnims
{
	public AnimationClip skid, land, swingDown, swingUp, plunge, defend, usePotion;
}

public class PlayerMob : Mob<PlayerMobAnims>
{
	public SpriteAnimNodes nodes { get; private set; }
	public Interactor interactor { get; private set; }

	public float maxAP = 100f;
	public Collider2DCallback meleeDamage;
	public float currentAP { get; protected set; }
	public bool isAPRecharging { get; private set; }

	private WeaponItem mainWeaponObject, offWeaponObject;
	private Item itemInUse;
	[System.NonSerialized]
	public bool hasShieldUp;
	private bool hasSecondChance = true;

	protected override void Start()
	{
		base.Start();

		nodes = GetComponent<SpriteAnimNodes>();
		interactor = GetComponentInChildren<Interactor>();

		meleeDamage.trigger2D.enabled = false;
	}

	protected override void Update()
	{
		base.Update();

		if (GameState.isPaused || isDead)
			return;

		// MOVEMENT

		moveInput = !ignoreMoveInput ? new Vector2(GameState.moveAxis.GetAxis(), 0f).normalized.x : 0f;

		if (isOnGround)
		{
			if (GameState.jumpAction.GetDown() && isOnGround && !ignoreMoveInput)
				velocity.y = jumpForce;
		}
		else
		{
			if (velocity.y > 0f && !ignoreMoveInput)
			{
				if (GameState.jumpAction.GetUp())
					velocity.y *= 0.7f;
			}

			if (!ignoreMoveInput && GameState.mainUseAction.GetDown() && mainWeaponObject && mainWeaponObject.GetType() == typeof(MeleeWeaponItem) && velocity.y > -jumpForce)
			{
				ignoreMoveInput = true;

				velocity.y *= 0f;
				velocity.y += 2.5f;

				animator.Play(anims.plunge, ((MeleeWeaponItem)mainWeaponObject).speed);
			}
		}

		if (currentAP <= 0f)
			isAPRecharging = true;

		if (isAPRecharging)
		{
			currentAP = Mathf.Clamp(currentAP + (5f * Time.deltaTime), 0f, maxAP);
			isAPRecharging = currentAP < maxAP;
		}

		// ITEMS

		if (mainWeaponObject)
		{
			mainWeaponObject.transform.position = nodes.GetPosition(0);
			mainWeaponObject.transform.eulerAngles = new Vector3(0f, 0f, nodes.GetAngle(0));

			if (GameState.mainUseAction.GetDown() && mainWeaponObject.CanUse(this))
			{
				mainWeaponObject.OnUse(this);
			}
			else if (GameState.mainUseAction.GetUp())
			{
				mainWeaponObject.OnEndUse(this);
			}
			else if (GameState.mainUseAction.GetHeld() && mainWeaponObject.CanUse(this))
			{
				mainWeaponObject.OnHoldUse(this);
			}
		}

		if (offWeaponObject)
		{
			offWeaponObject.transform.position = nodes.GetPosition(1);
			offWeaponObject.transform.eulerAngles = new Vector3(0f, 0f, nodes.GetAngle(1));

			if (GameState.offUseAction.GetDown() && offWeaponObject.CanUse(this))
			{
				offWeaponObject.OnUse(this);
			}
			else if (GameState.offUseAction.GetUp())
			{
				offWeaponObject.OnEndUse(this);
			}
			else if (GameState.offUseAction.GetHeld())
			{
				offWeaponObject.OnHoldUse(this);
			}
		}

		if (GameState.shortcut1UseAction.GetDown())
		{
			Item shortcutItem = PlayerState.instance.inventoryStock.GetItem(PlayerState.instance.shortcutItem1);

			if (shortcutItem && shortcutItem.CanUse(this))
			{
				shortcutItem.OnUse(this);
				itemInUse = shortcutItem;
			}
		}

		if (GameState.shortcut2UseAction.GetDown())
		{
			Item shortcutItem = PlayerState.instance.inventoryStock.GetItem(PlayerState.instance.shortcutItem2);

			if (shortcutItem && shortcutItem.CanUse(this))
			{
				shortcutItem.OnUse(this);
				itemInUse = shortcutItem;
			}
		}

		if (GameState.shortcut3UseAction.GetDown())
		{
			Item shortcutItem = PlayerState.instance.inventoryStock.GetItem(PlayerState.instance.shortcutItem3);

			if (shortcutItem && shortcutItem.CanUse(this))
			{
				shortcutItem.OnUse(this);
				itemInUse = shortcutItem;
			}
		}

		if (GameState.shortcut4UseAction.GetDown())
		{
			Item shortcutItem = PlayerState.instance.inventoryStock.GetItem(PlayerState.instance.shortcutItem4);

			if (shortcutItem && shortcutItem.CanUse(this))
			{
				shortcutItem.OnUse(this);
				itemInUse = shortcutItem;
			}
		}

		// INTERACT

		if (GameState.interactAction.GetDown() && !GameState.isMenuConsumingInput)
		{
			if (interactor.interactee != null && interactor.interactee.CanInteract(this))
			{
				interactor.interactee.OnInteract(this);
			}
		}
	}

	protected override void GetSetAnimation()
	{
		if (isDead || ignoreMoveInput)
			return;

		if (isOnGround)
		{
			if (moveInput != 0f)
			{
				if (velocity.x != 0 && moveInput != Mathf.Sign(velocity.x)) animator.Play(anims.skid);
				else animator.Play(anims.move, Mathf.Clamp(Mathf.Abs(velocity.x) / moveSpeed, 0.3f, 5f));
			}
			else animator.Play(anims.idle);
		}
		else animator.Play(velocity.y < 0f ? anims.fall : anims.jump);
	}

	public override IEnumerator OnDamage(GameObject instigator, GameObject causer, float damage, float knockback)
	{
		if (hasShieldUp)
		{
			float direction = Mathf.Sign(transform.position.x - causer.transform.position.x);

			if (direction != transform.localScale.x)
				yield break;
		}

		yield return base.OnDamage(instigator, causer, damage, knockback);
	}

	protected override IEnumerator OnDeath()
	{
		if (hasSecondChance)
		{
			Heal(1f);
			hasSecondChance = false;
			ignoreMoveInput = false;
			yield break;
		}

		if (mainWeaponObject)
			Destroy(mainWeaponObject.gameObject);

		if (offWeaponObject)
			Destroy(offWeaponObject.gameObject);

		yield return base.OnDeath();
	}

	protected override void OnLand()
	{
		ignoreMoveInput = true;
		AnimDisableMeleeDamage();
		animator.Play(anims.land);
	}

	public void EquipItem(Item item)
	{
		if (PlayerState.instance.inventoryStock.GetItem(item))
		{
			switch (((WeaponItem)item).wieldType)
			{
				case WieldType.MainHand:
					if (mainWeaponObject)
						Destroy(mainWeaponObject.gameObject);

					mainWeaponObject = Instantiate((WeaponItem)item, transform);
					mainWeaponObject.transform.position = nodes.GetPosition(0);
					mainWeaponObject.transform.eulerAngles = new Vector3(0f, 0f, nodes.GetAngle(0));
					mainWeaponObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
					break;

				case WieldType.OffHand:
					if (offWeaponObject)
						Destroy(offWeaponObject.gameObject);

					offWeaponObject = Instantiate((WeaponItem)item, transform);
					offWeaponObject.transform.position = nodes.GetPosition(1);
					offWeaponObject.transform.eulerAngles = new Vector3(0f, 0f, nodes.GetAngle(1));
					offWeaponObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
					break;

				case WieldType.TwoHand:
					if (mainWeaponObject)
						Destroy(mainWeaponObject.gameObject);

					if (offWeaponObject)
						Destroy(offWeaponObject.gameObject);
					break;
			}
		}
	}

	public void PrepareMeleeDamage(MeleeWeaponItem item)
	{
		((BoxCollider2D)meleeDamage.trigger2D).size = new Vector2(item.range, 1f);
		((BoxCollider2D)meleeDamage.trigger2D).offset = new Vector2(item.range / 2f, -0.1f);
		meleeDamage.trigger2D.enabled = false;
		meleeDamage.onTriggerEnter2D += item.DoMeleeDamage;
	}

	#region Animation

	private void AnimUseItem(int i)
	{
		if (itemInUse)
			itemInUse.OnAnimEvent(this, i);
	}

	private void AnimPlayLandEffect(Object effect)
	{
		Instantiate((GameObject)effect, nodes.GetPosition(2) + (Vector3.right * velocity.x * 0.1f), Quaternion.identity).transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		Instantiate((GameObject)effect, nodes.GetPosition(3) + (Vector3.right * velocity.x * 0.1f), Quaternion.identity).transform.localScale = transform.localScale;
	}

	private void AnimPlaySkidEffect(Object effect)
	{
		Instantiate((GameObject)effect, nodes.GetPosition(2) + (Vector3.right * velocity.x * 0.1f), Quaternion.identity).transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}

	private void AnimEnableMeleeDamage()
	{
		meleeDamage.trigger2D.enabled = true;
	}

	private void AnimDisableMeleeDamage()
	{
		meleeDamage.trigger2D.enabled = false;
		meleeDamage.ClearOnTriggerEnter2D();
	}

	#endregion
}
