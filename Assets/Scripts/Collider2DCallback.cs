using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Collider2DCallback : MonoBehaviour
{
	public Collider2D trigger2D { get; private set; }

	public delegate void OnTriggerEnter2DCallback(Collider2D collision);
	public event OnTriggerEnter2DCallback onTriggerEnter2D;

	public delegate void OnTriggerExit2DCallback(Collider2D collision);
	public event OnTriggerExit2DCallback onTriggerExit2D;

	public delegate void OnTriggerStay2DCallback(Collider2D collision);
	public event OnTriggerStay2DCallback onTriggerStay2D;

	private void Start()
	{
		Collider2D[] colliders = GetComponents<Collider2D>();

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].isTrigger)
			{
				trigger2D = GetComponent<Collider2D>();
				break;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (onTriggerEnter2D != null)
			onTriggerEnter2D(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (onTriggerExit2D != null)
			onTriggerExit2D(collision);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (onTriggerStay2D != null)
			onTriggerStay2D(collision);
	}

	public void ClearOnTriggerEnter2D()
	{
		onTriggerEnter2D = null;
	}

	public void ClearOnTriggerExit2D()
	{
		onTriggerExit2D = null;
	}

	public void ClearOnTriggerStay2D()
	{
		onTriggerStay2D = null;
	}
}
