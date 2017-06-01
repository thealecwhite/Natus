using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);

		if (IsInteractable())
			Select();
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		// Nothing here on purpose.
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		OnPointerClick(eventData);
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);

		base.OnPointerExit(new PointerEventData(EventSystem.current));
	}
}
