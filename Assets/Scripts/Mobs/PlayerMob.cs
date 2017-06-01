using System;
using System.Collections;

using UnityEngine;

using PowerTools;

[Serializable]
public class PlayerAnimations : Animations
{
	public AnimationClip skid, land, swingAttack, stabAttack, plungeAttack, deflect, drawBow, releaseBow, usePotion;
}

public class PlayerMob : Mob<PlayerAnimations>
{
	public SpriteAnimNodes nodes { get; private set; }
	public Interactor interactor { get; private set; }

	public float maxAP = 100f;
	public float currentAP { get; protected set; }
	public bool isAPRecharging { get; private set; }
	public delegate void OnDamaged(float damage, GameObject instigator, GameObject causer);
	public event OnDamaged onDamaged;

	[NonSerialized]
	public Item itemInUse;
	private WeaponItem mainWeaponObject, offWeaponObject;

	protected override void Start()
	{
		base.Start();

		nodes = GetComponent<SpriteAnimNodes>();
		interactor = GetComponentInChildren<Interactor>();
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

				animator.Play(animations.plungeAttack, ((MeleeWeaponItem)mainWeaponObject).speed);
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
		}

		if (GameState.shortcutUseAction.GetDown())
		{
			Item temp = PlayerState.instance.itemStock.GetItem(GameState.items.potion);

			if (temp && temp.CanUse(this))
				temp.OnUse(this);
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
			if (moveInput != 0f && !ignoreMoveInput)
			{
				if (velocity.x != 0 && moveInput != Mathf.Sign(velocity.x)) animator.Play(animations.skid);
				else animator.Play(animations.move, Mathf.Clamp(Mathf.Abs(velocity.x) / moveSpeed, 0.3f, 5f));
			}
			else animator.Play(animations.idle);
		}
		else animator.Play(velocity.y < 0f ? animations.fall : animations.jump);
	}

	public override IEnumerator OnDamage(float damage, GameObject instigator, GameObject causer)
	{
		if (!isDamageable)
			yield break;

		ignoreMoveInput = true;
		animator.Play(animations.hurt);
		velocity = new Vector2((instigator.transform.position - transform.position).normalized.x * -4f, 2f);

		AnimDisableDamage();
		AnimShowSecondaryWeapon();

		if (currentHP <= 1) currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
		else currentHP = Mathf.Clamp(currentHP - damage, 1, maxHP);

		onDamaged(damage, instigator, causer);

		if (isDead)
		{
			StartCoroutine(OnDeath());
			yield break;
		}

		renderer.color = Color.red;

		yield return new WaitForSeconds(0.2f);

		renderer.color = Color.white;

		ignoreMoveInput = false;

		yield return null;
	}

	protected override IEnumerator OnDeath()
	{
		if (mainWeaponObject)
			Destroy(mainWeaponObject.gameObject);

		if (offWeaponObject)
			Destroy(offWeaponObject.gameObject);

		return base.OnDeath();
	}

	protected override void OnLand()
	{
		ignoreMoveInput = true;
		animator.Play(animations.land);

		AnimDisableDamage();
		AnimShowSecondaryWeapon();
	}

	public void EquipItem(WeaponItem item, bool mainHand)
	{
		if (PlayerState.instance.itemStock.GetItem(item))
		{
			if (mainHand)
			{
				if (mainWeaponObject)
					Destroy(mainWeaponObject.gameObject);

				mainWeaponObject = Instantiate(item, transform);
				mainWeaponObject.transform.position = nodes.GetPosition(0);
				mainWeaponObject.transform.eulerAngles = new Vector3(0f, 0f, nodes.GetAngle(0));
				mainWeaponObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
			}
			else
			{
				if (offWeaponObject)
					Destroy(offWeaponObject.gameObject);

				offWeaponObject = Instantiate(item, transform);
				offWeaponObject.transform.position = nodes.GetPosition(1);
				offWeaponObject.transform.eulerAngles = new Vector3(0f, 0f, nodes.GetAngle(1));
				offWeaponObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
			}
		}
	}

	#region Animation

	private void AnimUseItem(int i)
	{
		if (itemInUse)
			itemInUse.OnAnimEvent(this, i);
	}

	private void AnimEnableMovement()
	{
		ignoreMoveInput = false;
	}

	private void AnimDisableMovement()
	{
		ignoreMoveInput = true;
	}

	private void AnimAddForceHorizontal(float amount)
	{
		velocity.x += amount * transform.localScale.x;
	}

	private void AnimAddForceVertical(float amount)
	{
		velocity.y += amount;
	}

	private void AnimPlayLandEffect(UnityEngine.Object effect)
	{
		Instantiate((GameObject)effect, nodes.GetPosition(2) + (Vector3.right * velocity.x * 0.1f) + (Vector3.right * 0.05f), Quaternion.identity).transform.localScale = transform.localScale;
		Instantiate((GameObject)effect, nodes.GetPosition(3) + (Vector3.right * velocity.x * 0.1f) + (Vector3.right * 0.05f), Quaternion.identity).transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}

	private void AnimPlaySkidEffect(UnityEngine.Object effect)
	{
		Instantiate((GameObject)effect, nodes.GetPosition(2) + (Vector3.right * velocity.x * 0.1f), Quaternion.identity).transform.localScale = transform.localScale;
	}

	private void AnimEnableDamage()
	{
		if (mainWeaponObject)
			mainWeaponObject.GetComponent<BoxCollider2D>().enabled = true;
	}

	private void AnimDisableDamage()
	{
		if (mainWeaponObject)
			mainWeaponObject.GetComponent<BoxCollider2D>().enabled = false;
	}

	private void AnimSetWeaponSortingOrder(int order)
	{
		if (mainWeaponObject)
			mainWeaponObject.GetComponent<SpriteRenderer>().sortingOrder = order;
	}

	private void AnimHideSecondaryWeapon()
	{
		if (offWeaponObject)
			offWeaponObject.GetComponent<SpriteRenderer>().enabled = false;
	}

	private void AnimShowSecondaryWeapon()
	{
		if (offWeaponObject)
			offWeaponObject.GetComponent<SpriteRenderer>().enabled = true;
	}

#endregion
}
