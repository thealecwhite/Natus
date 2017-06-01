using UnityEngine;
using PowerTools;

public class Effect : MonoBehaviour
{
	public SpriteAnim spriteAnim { get; protected set; }

	private void Start()
	{
		spriteAnim = GetComponent<SpriteAnim>();
	}

	protected virtual void Update()
	{
		if (!spriteAnim.IsPlaying())
			Destroy(gameObject);
	}
}
