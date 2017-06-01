using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
	public LayerMask collisionLayerMask;

	private RaycastHit2D[] hits = new RaycastHit2D[16];

	private void Start()
	{
		transform.position = target.position + offset;
	}

	private void Update()
	{
		if (!target)
			return;

		float vertical = Camera.main.orthographicSize;
		float horizontal = Camera.main.orthographicSize * Screen.width / Screen.height;

		Vector3 phantomPosition = target.position + offset;
		Vector3 differencePosition = phantomPosition - transform.position;

		for (int i = 0; i < Physics2D.BoxCastNonAlloc(transform.position, new Vector2(horizontal - 0.1f, vertical - 0.1f) * 2f, 0f, (phantomPosition - transform.position).normalized, hits, (phantomPosition - transform.position).magnitude + 0.1f, collisionLayerMask); i++)
		{
			if (hits[i].normal == Vector2.up || hits[i].normal == Vector2.down)
			{
				differencePosition.y = (hits[i].distance - 0.1f) * -hits[i].normal.x;
			}
			else if (hits[i].normal == Vector2.right || hits[i].normal == Vector2.left)
			{
				differencePosition.x = (hits[i].distance - 0.1f) * -hits[i].normal.y;
			}
		}

		transform.Translate(differencePosition);
	}
}
