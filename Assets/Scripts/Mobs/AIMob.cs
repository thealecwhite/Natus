using System;
using System.Collections;

using UnityEngine;

using MonsterLove.StateMachine;

public enum MobStates
{
	Idle, Chase, Defend, Attack
}

public class AIMob<TAnims, TStates> : Mob<TAnims> where TAnims : MobAnims where TStates : struct, IConvertible, IComparable
{
	public StateMachine<TStates> stateMachine { get; private set; }

	public readonly TStates states;
	public float viewDistance = 5f;

	protected Transform target;
	protected Vector2 home;

	protected override void Start()
	{
		base.Start();

		stateMachine = StateMachine<TStates>.Initialize(this);

		home = transform.position;
	}

	protected override void Update()
	{
		base.Update();

		if (GameState.isPaused || isDead)
			return;

		if (isOnGround)
		{
			if (Physics2D.Raycast(transform.position, Vector2.right * moveInput, Mathf.Abs(moveInput) * moveSpeed / 2f, controller.collisionLayerMask))
				velocity.y = jumpForce;
		}
	}

	protected override IEnumerator OnDeath()
	{
		// StopAllCoroutines();

		return base.OnDeath();
	}

	protected RaycastHit2D CheckForPlayerHit()
	{
		if (!GameState.player || GameState.player.isDead)
			return default(RaycastHit2D);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, (GameState.player.transform.position - transform.position).normalized, viewDistance, LayerMask.GetMask("Default", "Player"));

		if (!hit || !hit.transform.CompareTag("Player"))
			return default(RaycastHit2D);

		return hit;
	}

	protected bool MoveTo(Vector2 target, float distance)
	{
		if ((target - (Vector2)transform.position).magnitude > distance)
		{
			if (!ignoreMoveInput)
				moveInput = Mathf.Sign(target.x - transform.position.x);

			return false;
		}
		else
		{
			if (!ignoreMoveInput)
				moveInput = 0f;

			return true;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white * 0.5f;
		Gizmos.DrawWireSphere(transform.position, viewDistance);
	}
}
