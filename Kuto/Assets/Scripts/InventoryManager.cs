using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour {

	public static InventoryManager im;

	[SerializeField] Inventory inventory;
	[SerializeField] EquipmentPanel equipmentPanel;

	private void Awake()
	{
		im = this;
		inventory.OnItemRightClickedEvent += EquipFromInventory;
		equipmentPanel.OnItemRightClickedEvent += unequipFromEquipmentPanel;
	}

	// void Start()
	// {
	// 	Fill();
	// }

	public void Fill()
	{
		inventory.inventory.Clear();
		// System.Array.Clear(equipmentPanel.equipped,0,3);
		for (int i = 0; i < equipmentPanel.equipped.Length; i++)
		{
			equipmentPanel.equipped[i].Item = null;
		}
		for (int i = 0; i < GameControl.control.equippedItems.Count; i++)
		{	
			inventory.AddItem(GameControl.control.equippedItems[i]);
			EquipFromInventory(inventory.inventory[0]);
		}
		for (int i = 0; i < GameControl.control.inventoryItems.Count; i++)
		{
			inventory.AddItem(GameControl.control.inventoryItems[i]);
		}
	}

	public void SaveInventory()
	{
		GameControl.control.inventoryItems = inventory.inventory;
		List<EquippableItem> equipped = new List<EquippableItem>();
		foreach (var item in equipmentPanel.equipped)
		{
			if(item.Item != null)
				equipped.Add((EquippableItem)item.Item);
		}
		GameControl.control.equippedItems = equipped;		
	}

	public void EquipFromInventory(Item item)
	{
		if(item is EquippableItem)
		{
			Equip((EquippableItem)item);
		}
	}

	private void unequipFromEquipmentPanel(Item item)
	{
		if(item is EquippableItem)
		{
			Unequip((EquippableItem)item);
		}
	}

	public void Equip(EquippableItem item)
	{
		if(inventory.RemoveItem(item))
		{
			EquippableItem previousItem;
			if (equipmentPanel.AddItem(item, out previousItem))
			{
				if (previousItem != null)
				{
					inventory.AddItem(previousItem);
				}
			}else
			{
				inventory.AddItem(item);
			}
		}
	}


	public void Unequip(EquippableItem item)
	{
		if (!inventory.IsFull() && equipmentPanel.RemoveItem(item))
		{
			inventory.AddItem(item);
		}
	}

}
