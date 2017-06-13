using System;
using System.Collections;

using UnityEngine;

using PowerTools;

[Serializable]
public class MobAnims
{
	public AnimationClip idle, move, jump, fall, hurt, dead;
}

public class Mob<TAnim> : MonoBehaviour, IDamageable where TAnim : MobAnims
{
	public CharacterController2D controller { get; private set; }
	public SpriteAnim animator { get; private set; }
	public new SpriteRenderer renderer { get; private set; }

	public TAnim anims;
	public float moveSpeed = 3f;
	public float jumpForce = 5f;
	public float gravityScale = 1f;
	public float groundAccelerationTime = 0.025f, airAccelerationTime = 0.5f;
	public float knockbackScale = 1f;
	public float maxHP = 100f;
	public float currentHP { get; protected set; }
	public bool isOnGround { get { return controller.isOnGround; } }
	public bool isDead { get { return currentHP <= 0 || maxHP <= 0; } }
	public bool isDamageable { get { return !isDead; } }
	public delegate void OnDamaged(GameObject instigator, GameObject causer, float damage);
	public event OnDamaged onDamaged;

	[NonSerialized]
	public Vector2 velocity;
	[NonSerialized]
	public float moveInput;
	[NonSerialized]
	public bool ignoreMoveInput;
	private float smoothVelocity;
	private bool wasOnGround;

	protected virtual void Start()
	{
		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<SpriteAnim>();
		renderer = GetComponent<SpriteRenderer>();

		currentHP = maxHP;
	}

	protected virtual void Update()
	{
		if (GameState.isPaused || isDead)
			return;

		if (!ignoreMoveInput && moveInput != 0f)
		{
			if (velocity.x != 0) transform.localScale = new Vector3(Mathf.Sign(velocity.x), 1f, 1f);
			else transform.localScale = new Vector3(Mathf.Sign(moveInput), 1f, 1f);
		}

		if (isOnGround)
		{
			if (!wasOnGround)
			{
				wasOnGround = true;
				OnLand();
			}
		}
		else wasOnGround = false;
	}

	private void FixedUpdate()
	{
		velocity += Physics2D.gravity * gravityScale * Time.deltaTime;
		velocity.x = Mathf.SmoothDamp(velocity.x, !ignoreMoveInput ? moveInput * moveSpeed : 0f, ref smoothVelocity, isOnGround ? groundAccelerationTime : airAccelerationTime, 15f);

		controller.Move(velocity * Time.deltaTime);

		if (isOnGround || (controller.collision & CollisionFlags2D.Above) != 0)
			velocity.y = Physics2D.gravity.y * gravityScale * Time.deltaTime;

		if ((controller.collision & CollisionFlags2D.Left) != 0 || (controller.collision & CollisionFlags2D.Right) != 0)
			velocity.x = 0f;

		GetSetAnimation();
	}

	protected virtual void GetSetAnimation()
	{
		if (isDead || ignoreMoveInput)
			return;

		if (isOnGround)
		{
			if (moveInput != 0f) animator.Play(anims.move, Mathf.Clamp(Mathf.Abs(velocity.x) / moveSpeed, 0.3f, 5f));
			else animator.Play(anims.idle);
		}
		else animator.Play(velocity.y < 0f ? anims.fall : anims.jump);
	}

	protected virtual void OnLand()
	{
		// Apply damage or something
	}

	public virtual void Heal(float amount)
	{
		currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
	}

	public virtual IEnumerator OnDamage(GameObject instigator, GameObject causer, float damage, float knockback)
	{
		if (!isDamageable)
			yield break;

		ignoreMoveInput = true;
		currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
		velocity = new Vector2(knockback * Mathf.Sign(transform.position.x - causer.transform.position.x) * knockbackScale, knockback / 2f * knockbackScale);

		if (onDamaged != null)
			onDamaged(instigator, causer, damage);

		if (isDead)
		{
			StartCoroutine(OnDeath());
			yield break;
		}

		animator.Play(anims.hurt);

		renderer.color = Color.red;

		yield return new WaitForSeconds(0.2f);

		renderer.color = Color.white;

		ignoreMoveInput = false;
	}

	protected virtual IEnumerator OnDeath()
	{
		ignoreMoveInput = true;

		animator.Play(anims.dead);

		renderer.color = Color.red;

		yield return new WaitForSeconds(0.1f);

		renderer.color = Color.white;
	}

	#region Animation

	private void AnimDestroySelf()
	{
		Destroy(gameObject);
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

	#endregion
}
