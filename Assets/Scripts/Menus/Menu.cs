using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	public Selectable defaultSelect;
	private static Selectable lastSelect;

	public virtual void Open()
	{
		gameObject.SetActive(true);

		if (lastSelect && lastSelect.transform.IsChildOf(transform)) SetSelection(lastSelect);
		else if (defaultSelect) SetSelection(defaultSelect);
	}

	public virtual void Open(Selectable select)
	{
		gameObject.SetActive(true);
		SetSelection(select);
	}

	public virtual void Close()
	{
		gameObject.SetActive(false);
	}

	protected static void SetSelection(Selectable select)
	{
		if (EventSystem.current.currentSelectedGameObject)
			lastSelect = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

		select.Select();
		select.OnSelect(new BaseEventData(EventSystem.current));
	}
}
