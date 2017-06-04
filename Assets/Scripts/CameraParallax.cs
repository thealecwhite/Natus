using System;

using UnityEngine;

using UnityToolbag;

public class CameraParallax : MonoBehaviour
{
	[Serializable]
	public struct ParallaxLayer
	{
		public Sprite sprite;
		[SortingLayer]
		public int sortingLayer;
		public int orderInLayer;
		[Range(0f, 5f)]
		public float speed;

		[NonSerialized]
		public Transform start, other;
		[NonSerialized]
		public int startSide, otherSide;
	}

	[SortableArray]
	public ParallaxLayer[] parallaxLayers;

	private Vector3 lastPosition;

	private void Start()
	{
		for (int i = 0; i < parallaxLayers.Length; i++)
		{
			parallaxLayers[i].start = new GameObject(string.Format("Parallax Layer ({0})", i), typeof(SpriteRenderer)).transform;
			parallaxLayers[i].start.position = (Vector2)transform.position;
			parallaxLayers[i].start.gameObject.hideFlags = HideFlags.HideInHierarchy;

			{
				SpriteRenderer renderer = parallaxLayers[i].start.GetComponent<SpriteRenderer>();
				renderer.sprite = parallaxLayers[i].sprite;
				renderer.sortingLayerID = parallaxLayers[i].sortingLayer;
				renderer.sortingOrder = parallaxLayers[i].orderInLayer;
			}

			if (parallaxLayers[i].speed > 0f)
			{
				parallaxLayers[i].other = new GameObject(string.Format("Parallax Layer ({0}) Other", i), typeof(SpriteRenderer)).transform;
				parallaxLayers[i].other.position = (Vector2)transform.position;
				parallaxLayers[i].other.gameObject.hideFlags = HideFlags.HideInHierarchy;

				{
					SpriteRenderer renderer = parallaxLayers[i].other.GetComponent<SpriteRenderer>();
					renderer.sprite = parallaxLayers[i].sprite;
					renderer.sortingLayerID = parallaxLayers[i].sortingLayer;
					renderer.sortingOrder = parallaxLayers[i].orderInLayer;
				}
			}
		}
	}

	private void LateUpdate()
	{
		float cameraHeight = Camera.main.orthographicSize;
		float cameraWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
		float cameraLeft = transform.position.x - cameraWidth;
		float cameraRight = transform.position.x + cameraWidth;

		for (int i = 0; i < parallaxLayers.Length; i++)
		{
			ParallaxLayer layer = parallaxLayers[i];

			if (layer.speed <= 0f)
			{
				layer.start.position = (Vector2)transform.position;
				continue;
			}

			Vector2 parallax = new Vector2((lastPosition - transform.position).x * -layer.speed, (lastPosition - transform.position).y * -0.95f);

			layer.start.position = (Vector2)layer.start.position + parallax;
			layer.other.position = (Vector2)layer.other.position + parallax;

			float startLeft = layer.start.position.x - layer.sprite.bounds.extents.x, startRight = layer.start.position.x + layer.sprite.bounds.extents.x;
			float otherLeft = layer.start.position.x - layer.sprite.bounds.extents.x, otherRight = layer.start.position.x + layer.sprite.bounds.extents.x;

			// Start side
			{
				if (cameraLeft > startRight)
				{
					layer.start.position = new Vector2(otherRight + layer.sprite.bounds.extents.x, layer.start.position.y);
				}
				else if (cameraRight < startLeft)
				{
					layer.start.position = new Vector2(otherLeft - layer.sprite.bounds.extents.x, layer.start.position.y);
				}
			}

			// Other side
			{
				if (cameraLeft > otherLeft)
				{
					layer.other.position = new Vector2(startRight + layer.sprite.bounds.extents.x, layer.other.position.y);
				}
				else if (cameraLeft < otherLeft)
				{
					layer.other.position = new Vector2(startLeft - layer.sprite.bounds.extents.x, layer.other.position.y);
				}
			}
		}

		lastPosition = transform.position;
	}
}
