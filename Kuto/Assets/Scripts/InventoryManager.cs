using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

	public static InventoryManager im;

	public Inventory inventory;
	public EquipmentPanel equipmentPanel;
	[SerializeField] List<StatDisplay> stats;
	public GameObject cooldownParent;
	private List<Image> cooldowns = new List<Image>();
	[SerializeField] List<Sprite> icons;

	private void Awake()
	{
		im = this;
		inventory.OnItemRightClickedEvent += EquipFromInventory;
		inventory.OnItemLeftClickedEvent += Shop.instance.Click;
		inventory.OnItemMobileClickedEvent += MobileTouch;
		equipmentPanel.OnItemLeftClickedEvent += MobileTouch;
		equipmentPanel.OnItemRightClickedEvent += unequipFromEquipmentPanel;
	}

	void Start()
	{
		foreach (Transform child in cooldownParent.transform)
		{
			cooldowns.Add(child.GetComponent<Image>());
		}
		Fill();
		RefreshCooldowns();
	}

	public void Fill()
	{
		inventory.inventory.Clear();
		
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

		StatDisplay();
	}

	public void StatDisplay()
	{
		stats[0].ValueText.text = GameControl.control.GetLevel().ToString();
		stats[1].ValueText.text = GameControl.control.vitality.ToString();
		stats[2].ValueText.text = GameControl.control.strength.ToString();
		stats[3].ValueText.text = GameControl.control.gold.ToString();
		stats[4].ValueText.text = GameControl.control.shards.ToString();
	}

	public void MobileTouch(Item item)
	{
		switch (Shop.instance.mode)
		{
			case 0:
				EquipFromInventory(item);
				// unequipFromEquipmentPanel(item);
				break;
			case 1:
				Shop.instance.Upgrade(item);
				break;
			case 2:
				Shop.instance.Sell(item);
				break;
			default: break;
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

		RefreshCooldowns();	
	}

	public void EquipFromInventory(Item item)
	{
		if(item is EquippableItem)
		{
			Equip((EquippableItem)item);
			if (GameControl.control.loaded)
			{		
				RefreshCooldowns();
			}
		}
	}

	private void unequipFromEquipmentPanel(Item item)
	{
		if(item is EquippableItem)
		{
			Unequip((EquippableItem)item);
			if (GameControl.control.loaded)
			{		
				RefreshCooldowns();
			}
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

	public void RefreshCooldowns()
	{
		for (int i = 0; i < equipmentPanel.equipmentSlots.Length -1; i++)
		{
			EquippableItem item = (EquippableItem)equipmentPanel.equipmentSlots[i].Item;
			if (item == null)
			{
				cooldowns[i].sprite = null;
				cooldowns[i].GetComponent<Cooldown>().skill = 0;

				Color newColor = new Color(.35f, .35f, .35f, 0);
            	cooldowns[i].GetComponent<Image>().color = newColor;
			}
			else
			{
				cooldowns[i].sprite = icons[item.skillID];
				cooldowns[i].GetComponent<Cooldown>().skill = item.skillID;

				Color newColor = new Color(255f, 255f, 255f, 255f);
            	cooldowns[i].GetComponent<Image>().color = newColor;
			}
		}
		GameControl.control.SaveCooldowns();
	}

}
