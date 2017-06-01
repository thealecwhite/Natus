using System.Collections.Generic;

using UnityEngine;

public class Interactor : MonoBehaviour
{
	public IInteractable interactee { get; private set; }

	private List<IInteractable> interactables = new List<IInteractable>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		IInteractable interactable = interactables.Find(i => ((MonoBehaviour)i).transform == collision.transform || collision.transform.IsChildOf(((MonoBehaviour)i).transform));

		if (interactable == null)
		{
			interactable = collision.GetComponent<IInteractable>();

			if (interactable == null && collision.transform != transform.root)
				interactable = collision.GetComponentInParent<IInteractable>();

			if (interactable != null)
			{
				interactables.Add(interactable);
				interactable.OnInteractEnter();
				interactee = GetClosestInteractable();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		IInteractable interactable = interactables.Find(i => ((MonoBehaviour)i).transform == collision.transform || collision.transform.IsChildOf(((MonoBehaviour)i).transform));

		if (interactable != null)
		{
			interactables.Remove(interactable);
			interactable.OnInteractExit();
			interactee = GetClosestInteractable();
		}
	}

	private IInteractable GetClosestInteractable()
	{
		IInteractable result = null;
		float shortestDistance = 0f;

		for (int i = 0; i < interactables.Count; i++)
		{
			float currentDistance = (((MonoBehaviour)interactables[i]).transform.position - transform.position).magnitude;

			if (shortestDistance == 0f || currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				result = interactables[i];
			}
		}

		return result;
	}
}
