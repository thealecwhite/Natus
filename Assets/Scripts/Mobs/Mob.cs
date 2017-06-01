using System;
using System.Collections;

using UnityEngine;

using PowerTools;

[Serializable]
public class Animations
{
	public AnimationClip idle, move, jump, fall, hurt;
}

public class Mob<TAnim> : MonoBehaviour, IDamageable where TAnim : Animations
{
	public CharacterController2D controller { get; private set; }
	public SpriteAnim animator { get; private set; }
	public new SpriteRenderer renderer { get; private set; }

	public float moveSpeed = 3f;
	public float jumpForce = 5f;
	public float gravityScale = 1f;
	public float groundAccelerationTime = 0.025f, airAccelerationTime = 0.5f;
	public TAnim animations;
	public float maxHP = 100f;
	public float currentHP { get; protected set; }
	public bool isOnGround { get { return (controller.collision & CollisionFlags2D.Below) != 0; } }
	public bool isDead { get { return currentHP <= 0 || maxHP <= 0; } }
	public bool isDamageable { get { return !isDead; } }

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
		if (isDead)
			return;

		if (isOnGround)
		{
			if (moveInput != 0f && !ignoreMoveInput) animator.Play(animations.move, Mathf.Clamp(Mathf.Abs(velocity.x) / moveSpeed, 0.1f, 3f));
			else animator.Play(animations.idle);
		}
		else animator.Play(velocity.y < 0f ? animations.fall : animations.jump);
	}

	protected virtual void OnLand()
	{
		// Apply damage or something
	}

	public virtual void Heal(float amount)
	{
		currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
	}

	public virtual IEnumerator OnDamage(float damage, GameObject instigator, GameObject causer)
	{
		if (!isDamageable)
			yield break;

		ignoreMoveInput = true;
		animator.Play(animations.hurt);
		velocity = new Vector2((instigator.transform.position - transform.position).normalized.x * -4f, 2f);

		currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);

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

	protected virtual IEnumerator OnDeath()
	{
		ignoreMoveInput = true;

		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

		int i = 0;

		while (i < 3)
		{
			spriteRenderer.color = Color.red;

			yield return new WaitForSeconds(0.1f);

			spriteRenderer.color = Color.white;

			yield return new WaitForSeconds(0.1f);

			i++;
		}

		Destroy(gameObject);

		yield return null;
	}
}
