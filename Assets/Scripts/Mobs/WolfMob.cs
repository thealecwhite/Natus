using System.Collections;

using UnityEngine;

[System.Serializable]
public class WolfMobAnims : MobAnims
{
	public AnimationClip sleep;
}

public class WolfMob : AIMob<WolfMobAnims>
{
	public BoxCollider2D pounceDamager;

	private Task followTask;
	private Task returnHomeTask;
	private bool isAwake;

	protected override void GetSetAnimation()
	{
		if (!isAwake)
		{
			animator.Play(animations.sleep);
			return;
		}

		base.GetSetAnimation();
	}

	protected override IEnumerator AI()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();

			RaycastHit2D hit = CheckForPlayerHit();

			if (hit)
			{
				isAwake = true;

				if (returnHomeTask != null && returnHomeTask.Running)
					returnHomeTask.Stop();

				if (followTask == null || !followTask.Running)
				{
					followTask = Task.Get(WaitToMoveTo(hit.transform, 2f, Random.Range(0.25f, 0.75f)));
					followTask.Finished += OnFollowTaskFinished;
				}
			}
			else
			{
				if ((homePosition - (Vector2)transform.position).magnitude > 1f)
				{
					if (returnHomeTask == null || !returnHomeTask.Running)
					{
						returnHomeTask = Task.Get(WaitToMoveTo(homePosition, 1f, Random.Range(1f, 2f)));
					}
				}
			}
		}
	}

	private void OnFollowTaskFinished(bool manual)
	{
		if (!this)
			return;

		followTask.Finished -= OnFollowTaskFinished;

		StartCoroutine(Attack());
	}

	private IEnumerator Attack()
	{
		if (ignoreMoveInput || pounceDamager.enabled || !GameState.player)
			yield break;

		float direction = Mathf.Sign((GameState.player.transform.position - transform.position).x);

		transform.localScale = new Vector3(direction, 1f, 1f);
		ignoreMoveInput = true;

		yield return new WaitForSeconds(0.5f);

		if (!GameState.player)
		{
			ignoreMoveInput = false;
			yield break;
		}

		if (isOnGround)
		{
			pounceDamager.enabled = true;
			velocity = new Vector2(direction * 8f, 3f);
		}

		yield return new WaitForSeconds(0.5f);

		pounceDamager.enabled = false;
		ignoreMoveInput = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.isTrigger)
			return;

		IDamageable damageable = collision.GetComponent<IDamageable>();

		if (damageable != null)
			((MonoBehaviour)damageable).StartCoroutine(damageable.OnDamage(25, gameObject, gameObject));
	}
}
