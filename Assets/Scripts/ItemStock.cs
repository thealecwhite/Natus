using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct ItemStock
{
	public ItemStock(int capacity)
	{
		items = new List<Item>(capacity);
	}

	[SerializeField]
	private List<Item> items;

	public T GetItem<T>(T item) where T : Item
	{
		return (T)items.Find(_item => _item == item);
	}

	public void AddItem(params Item[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (this.items.Count < this.items.Capacity)
			{
				if (this.items.FindAll(_item => _item == items[i]).Count < 99)
				{
					this.items.Add(items[i]);
				}
			}
		}
	}

	public void RemoveItem(Item item)
	{
		items.Remove(item);
	}
}
