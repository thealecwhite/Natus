using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct ItemStock
{
	public ItemStock(bool ignoreItemLimit)
	{
		items = new List<Item>();
		this.ignoreItemLimit = ignoreItemLimit;
	}

	[SerializeField]
	private List<Item> items;
	private bool ignoreItemLimit;

	public T GetItem<T>(T item) where T : Item
	{
		return (T)items.Find(_item => _item == item);
	}

	public bool AddItem(params Item[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (!ignoreItemLimit && this.items.FindAll(_item => _item == items[i]).Count < items[i].stockLimit)
			{
				this.items.Add(items[i]);
			}
			else return false;
		}

		return true;
	}

	public void RemoveItem(Item item)
	{
		items.Remove(item);
	}
}
