using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
	public GameObject selectionOutline;
	bool overridePlayerInput;

	private Canvas canvas;

	private void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	private void Update()
	{
		if (EventSystem.current.currentSelectedGameObject)
		{
			selectionOutline.SetActive(true);

			if (selectionOutline.transform.parent != EventSystem.current.currentSelectedGameObject.transform)
				selectionOutline.transform.SetParent(EventSystem.current.currentSelectedGameObject.transform, false);
		}
		else selectionOutline.SetActive(false);

		if (Screen.height <= 320) canvas.scaleFactor = 1;
		else if (Screen.height <= 480) canvas.scaleFactor = 2;
		else if (Screen.height <= 720) canvas.scaleFactor = 3;
		else canvas.scaleFactor = 4;
	}
}
