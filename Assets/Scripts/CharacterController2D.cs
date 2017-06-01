using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
	private struct RaycastOrigins
	{
		public Vector2 bottomLeft, bottomRight;
		public Vector2 topLeft, topRight;
	}

	public new BoxCollider2D collider { get; private set; }
	public CollisionFlags2D collision { get; private set; }

	[Range(2, 10)]
	public int horizontalRayCount = 3, verticalRayCount = 2;
	public LayerMask collisionLayerMask;

	private RaycastOrigins raycastOrigins;
	private float horizontalRaySpacing, verticalRaySpacing;
	private const float skinWidth = 0.015f;

	private void Start()
	{
		collider = GetComponent<BoxCollider2D>();
	}

	public void Move(Vector2 velocity)
	{
		collision = CollisionFlags2D.None;

		Bounds raycastBounds = collider.bounds;
		raycastBounds.Expand(skinWidth * -2f);

		UpdateRaycastOrigins(raycastBounds);
		CalculateRaySpacing(raycastBounds);

		if (velocity.x != 0f) CheckForHorizontalCollisions(ref velocity);
		if (velocity.y != 0f) CheckForVerticalCollisions(ref velocity);

		transform.Translate(velocity);
	}

	private void CheckForHorizontalCollisions(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = direction == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLength, collisionLayerMask);

			// Debug.DrawRay(rayOrigin, Vector2.right * direction * rayLength * 2f, Color.red);

			if (hit)
			{
				velocity.x = (hit.distance - skinWidth) * direction;
				rayLength = hit.distance;

				if (direction == -1) collision |= CollisionFlags2D.Left;
				else if (direction == 1) collision |= CollisionFlags2D.Right;
			}
		}
	}

	private void CheckForVerticalCollisions(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = direction == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * direction, rayLength, collisionLayerMask);

			// Debug.DrawRay(rayOrigin, Vector2.up * direction * rayLength * 2f, Color.blue);

			if (hit)
			{
				velocity.y = (hit.distance - skinWidth) * direction;
				rayLength = hit.distance;

				if (direction == -1) collision |= CollisionFlags2D.Below;
				else if (direction == 1) collision |= CollisionFlags2D.Above;
			}
		}
	}

	private void UpdateRaycastOrigins(Bounds bounds)
	{
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	private void CalculateRaySpacing(Bounds bounds)
	{
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
}
