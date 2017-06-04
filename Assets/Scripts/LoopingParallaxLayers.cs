using System;

using UnityEngine;

using UnityToolbag;

public class LoopingParallaxLayers : MonoBehaviour
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
		public Transform transform;
	}

	[SortableArray]
	public ParallaxLayer[] parallaxLayers;

	public Sprite skySprite, parallaxSprite;
	private Transform sky, leftParallax, rightParallax;
	private SpriteRenderer skyRenderer, leftParallaxRenderer, rightParallaxRenderer;
	private int rightParallaxSide, leftParallaxSide;
	private Vector3 lastPosition;

	private void Start()
	{
		for (int i = 0; i < parallaxLayers.Length; i++)
		{
			parallaxLayers[i].transform = new GameObject(string.Format("Parallax Layer ({0})", i), typeof(SpriteRenderer)).transform;
			parallaxLayers[i].transform.position = (Vector2)transform.position;

			{
				SpriteRenderer renderer = parallaxLayers[i].transform.GetComponent<SpriteRenderer>();
				renderer.sprite = parallaxLayers[i].sprite;
				renderer.sortingLayerID = parallaxLayers[i].sortingLayer;
				renderer.sortingOrder = parallaxLayers[i].orderInLayer;
			}
		}

		sky = new GameObject("Sky Background", typeof(SpriteRenderer)).transform;
		leftParallax = new GameObject("Left Parallax", typeof(SpriteRenderer)).transform;
		rightParallax = new GameObject("Right Parallax", typeof(SpriteRenderer)).transform;

		{
			skyRenderer = sky.GetComponent<SpriteRenderer>();
			skyRenderer.sprite = skySprite;
			skyRenderer.sortingOrder = -10;
		}

		{
			leftParallaxRenderer = leftParallax.GetComponent<SpriteRenderer>();
			leftParallaxRenderer.sprite = parallaxSprite;
			leftParallaxRenderer.sortingOrder = -5;
		}

		{
			rightParallaxRenderer = rightParallax.GetComponent<SpriteRenderer>();
			rightParallaxRenderer.sprite = parallaxSprite;
			rightParallaxRenderer.sortingOrder = -5;
		}

		skyRenderer.sortingLayerName = leftParallaxRenderer.sortingLayerName = rightParallaxRenderer.sortingLayerName = "Background";
		sky.gameObject.hideFlags = leftParallax.gameObject.hideFlags = rightParallax.gameObject.hideFlags = HideFlags.HideInHierarchy;

		leftParallax.position = new Vector2(transform.position.x, 0f);
		rightParallax.position = new Vector2(transform.position.x, 0f);
	}

	private void LateUpdate()
	{
		float vertical = Camera.main.orthographicSize;
		float horizontal = Camera.main.orthographicSize * Screen.width / Screen.height;

		Vector2 parallax = new Vector2((lastPosition - transform.position).x * -0.75f, (lastPosition - transform.position).y * -0.95f);

		sky.position = new Vector3(transform.position.x, transform.position.y);

		leftParallax.position = new Vector3(leftParallax.position.x + parallax.x, leftParallax.position.y + parallax.y);
		rightParallax.position = new Vector3(rightParallax.position.x + parallax.x, rightParallax.position.y + parallax.y);

		lastPosition = transform.position;

		// Left parallax looping

		if ((transform.position.x - horizontal) < (rightParallax.position.x - rightParallaxRenderer.sprite.bounds.size.x / 2f) + 0.05f)
		{
			float temp = (rightParallax.position.x - rightParallaxRenderer.sprite.bounds.size.x / 2f) - (leftParallaxRenderer.sprite.bounds.size.x / 2f);

			if (leftParallaxSide != -1)
			{
				leftParallax.position = new Vector3(temp, leftParallax.position.y);
				leftParallaxSide = -1;
			}
		}
		else if ((transform.position.x + horizontal) > (rightParallax.position.x - rightParallaxRenderer.sprite.bounds.size.x / 2f) - 0.05f)
		{
			float temp = (rightParallax.position.x + rightParallaxRenderer.sprite.bounds.size.x / 2f) + (leftParallaxRenderer.sprite.bounds.size.x / 2f);

			if (leftParallaxSide != 1)
			{
				leftParallax.position = new Vector3(temp, leftParallax.position.y);
				leftParallaxSide = 1;
			}
		}

		// Right parallax looping

		if ((transform.position.x - horizontal) < (leftParallax.position.x - leftParallaxRenderer.sprite.bounds.size.x / 2f) + 0.05f)
		{
			float temp = (leftParallax.position.x - leftParallaxRenderer.sprite.bounds.size.x / 2f) - (rightParallaxRenderer.sprite.bounds.size.x / 2f);

			if (rightParallaxSide != -1)
			{
				rightParallax.position = new Vector3(temp, rightParallax.position.y);
				rightParallaxSide = -1;
			}
		}
		else if ((transform.position.x + horizontal) > (leftParallax.position.x - leftParallaxRenderer.sprite.bounds.size.x / 2f) - 0.05f)
		{
			float temp = (leftParallax.position.x + leftParallaxRenderer.sprite.bounds.size.x / 2f) + (rightParallaxRenderer.sprite.bounds.size.x / 2f);

			if (rightParallaxSide != 1)
			{
				rightParallax.position = new Vector3(temp, rightParallax.position.y);
				rightParallaxSide = 1;
			}
		}
	}
}
