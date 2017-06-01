using UnityEngine;

using PowerTools;

public class Window : MonoBehaviour
{
	public AnimationClip openClip, closeClip;
	public delegate void OnOpenFinished();
	public event OnOpenFinished onOpenFinished;
	public delegate void OnCloseFinished();
	public event OnCloseFinished onCloseFinished;

	private SpriteAnim spriteAnim;
	private Vector2 defaultSizeDelta;

	private void Start()
	{
		defaultSizeDelta = ((RectTransform)transform).sizeDelta;
	}

	public void HideAllChildren()
	{
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(i).gameObject.SetActive(false);
	}

	public void SetChildrenOfParentActive(Transform parent)
	{
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(i).gameObject.SetActive(transform.GetChild(i) == parent);
	}

	public void SetSizeDeltaScale(Vector2 scale)
	{
		((RectTransform)transform).sizeDelta = new Vector2(defaultSizeDelta.x * scale.x, defaultSizeDelta.y * scale.y);
	}

	public void PlayOpenAnimation()
	{
		if (!spriteAnim)
			spriteAnim = GetComponent<SpriteAnim>();

		spriteAnim.Play(openClip);
	}

	public void PlayCloseAnimation()
	{
		if (!spriteAnim)
			spriteAnim = GetComponent<SpriteAnim>();

		spriteAnim.Play(closeClip);
	}

	#region Animations

	private void AnimSetSizeDeltaXScale(float scale)
	{
		SetSizeDeltaScale(new Vector2(scale, 1f));
	}

	private void AnimSetSizeDeltaYScale(float scale)
	{
		SetSizeDeltaScale(new Vector2(1f, scale));
	}

	private void AnimWindowOpened()
	{
		onOpenFinished();
	}

	private void AnimWindowClosed()
	{
		onCloseFinished();
	}

#endregion
}
