using System.Collections;

using UnityEngine;

[System.Serializable]
public class WolfMobAnims : MobAnims
{
	public AnimationClip sleep;
}

public class WolfMob : AIMob<WolfMobAnims, MobStates>
{
	public BoxCollider2D attackTrigger;

	private bool isAwake;

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
			isAwake = true;
			target = playerHit.transform;
			stateMachine.ChangeState(MobStates.Chase);
		}
		else MoveTo(home, 0.5f);
	}

	private IEnumerator Chase_Enter()
	{
		yield return new WaitForSeconds(0.5f);
	}

	private void Chase_FixedUpdate()
	{
		if (isDead)
			return;

		if (target)
		{
			if (MoveTo(target.position, 2f))
				stateMachine.ChangeState(MobStates.Attack);
		}
		else stateMachine.ChangeState(MobStates.Idle);
	}

	private IEnumerator Attack_Enter()
	{
		if (isDead || !target)
		{
			stateMachine.ChangeState(MobStates.Chase);
			yield break;
		}

		float direction = Mathf.Sign((GameState.player.transform.position - transform.position).x);

		transform.localScale = new Vector3(direction, 1f, 1f);
		ignoreMoveInput = true;

		yield return new WaitForSeconds(0.5f);

		if (isDead || !target)
		{
			ignoreMoveInput = false;
			stateMachine.ChangeState(MobStates.Chase);
			yield break;
		}

		if (isOnGround)
		{
			attackTrigger.enabled = true;
			velocity = new Vector2(direction * 8f, 3f);

			yield return new WaitForSeconds(1f);
		}

		attackTrigger.enabled = false;
		ignoreMoveInput = false;
		stateMachine.ChangeState(MobStates.Chase);
	}

	protected override void GetSetAnimation()
	{
		if (!isAwake)
		{
			animator.Play(anims.sleep);
			return;
		}

		base.GetSetAnimation();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.isTrigger)
			return;

		IDamageable damageable = collision.GetComponent<IDamageable>();

		if (damageable != null)
			((MonoBehaviour)damageable).StartCoroutine(damageable.OnDamage(gameObject, gameObject, 25, 3f));
	}
}
