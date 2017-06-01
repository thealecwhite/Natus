using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasGrid : MonoBehaviour
{
#if UNITY_EDITOR
	public Color gridColor = new Color(1, 1, 1, 0.25f);

	private Canvas canvas;
	private RectTransform rectTransform;

	public void OnDrawGizmos()
	{
		if (Selection.activeTransform && Selection.activeTransform.IsChildOf(transform))
		{
			if (!canvas)
				canvas = GetComponent<Canvas>();

			if (!rectTransform)
				rectTransform = GetComponent<RectTransform>();

			Rect canvasRect = new Rect(transform.position - (Vector3)rectTransform.sizeDelta / 2 * rectTransform.localScale.x, rectTransform.sizeDelta * rectTransform.localScale.x);

			Gizmos.color = gridColor;

			for (int x = 0; x < rectTransform.sizeDelta.x; x++)
				Gizmos.DrawLine(new Vector3(canvasRect.xMin + (x * rectTransform.localScale.x), canvasRect.yMin, transform.position.z), new Vector3(canvasRect.xMin + (x * rectTransform.localScale.x), canvasRect.yMax, transform.position.z));

			for (int y = 0; y < rectTransform.sizeDelta.y; y++)
				Gizmos.DrawLine(new Vector3(canvasRect.xMin, canvasRect.yMin + (y * rectTransform.localScale.y), transform.position.z), new Vector3(canvasRect.xMax, canvasRect.yMin + (y * rectTransform.localScale.y), transform.position.z));
		}
	}
#endif
}
