using System.Collections;

using UnityEngine;

public class AIMob<TAnim> : Mob<TAnim> where TAnim : MobAnims
{
	public float viewDistance = 5f;

	protected Vector2 homePosition;

	protected override void Start()
	{
		base.Start();

		homePosition = transform.position;

		StartCoroutine(AI());
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
		StopAllCoroutines();

		return base.OnDeath();
	}

	protected virtual IEnumerator AI()
	{
		yield return null;
	}

	protected RaycastHit2D CheckForPlayerHit()
	{
		if (!GameState.player || GameState.player.isDead)
			return default(RaycastHit2D);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, (GameState.player.transform.position - transform.position).normalized, viewDistance, LayerMask.GetMask("Default", "Player"));

		if (hit && !hit.transform.CompareTag("Player"))
			return default(RaycastHit2D);

		return hit;
	}

	protected IEnumerator WaitToMoveTo(Vector3 target, float acceptDistance, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		if (!this)
			yield break;

		while ((target - transform.position).magnitude > acceptDistance)
		{
			yield return new WaitForFixedUpdate();

			if (!this)
				yield break;

			moveInput = Mathf.Sign(target.x - transform.position.x);
		}

		moveInput = 0;

		yield return null;
	}

	protected IEnumerator WaitToMoveTo(Transform target, float acceptDistance, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		if (!this || !target)
			yield break;

		while ((target.position - transform.position).magnitude > acceptDistance)
		{
			yield return new WaitForFixedUpdate();

			if (!this || !target)
				yield break;

			moveInput = Mathf.Sign(target.position.x - transform.position.x);
		}

		moveInput = 0;

		yield return null;
	}
}
