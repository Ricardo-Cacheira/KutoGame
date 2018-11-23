using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	[SerializeField] List<Item> items;
	public List<Item> inventory{
		get{ return items; }
	}
	[SerializeField] Transform itemsParent;
	public ItemSlot[] itemSlots;

	public event Action<Item> OnItemRightClickedEvent;

	private void Awake()
	{
		for (int i = 0; i < itemSlots.Length; i++)
		{
			itemSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
		}
	}

	private void OnValidate()
	{
		//Only called on editor and triggers when script is loaded or change one of its values on the editor
		//used this to fill the itemSlots with all the item slots in the hierarchy
		if(itemsParent != null)
			itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();

		RefreshUI();
	}

	private void RefreshUI()
	{
		int i = 0;
		for (; i < items.Count && i < itemSlots.Length; i++)
		{
			itemSlots[i].Item = items[i];
		}

		for (; i < itemSlots.Length; i++)
		{
			itemSlots[i].Item = null;
		}
	}

	public bool AddItem(Item item)
	{
		if(IsFull())
			return false;

		items.Add(item);
		RefreshUI();
		return  true;
	}


	public bool RemoveItem(Item item)
	{
		if(items.Remove(item))
		{
			RefreshUI();
			return true;
		}

		return false;
	}

	public bool IsFull()
	{
		return items.Count >= itemSlots.Length;

	}
}
