using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

	public static Shop instance;

	public float shardFactor;
	public float goldFactor;
	public int shardBase;
	public int goldBase;

    private Text text;
    private Text cost;
    private Text level;
    [SerializeField] GameObject buttons;
	[SerializeField] List<StatDisplay> resources;

    public int mode; //0- none //1-upgrade //2-sell

	void Awake()
	{
		instance = this;
	}

    public void Click(Item item)
    {
		if(mode == 1)
		{
			Upgrade(item);
		}else if (mode == 2)
		{
			Sell(item);
		}
    }

    void Start()
	{
        cost = transform.GetChild(2).GetComponent<Text>();
        level = transform.GetChild(3).GetComponent<Text>();
        text = transform.GetChild(0).GetComponent<Text>();
        text.text = @"What can I"+System.Environment.NewLine+" help you with?";
		mode = 0;
		shardBase = 5;
		goldBase = 150;
		shardFactor = 1f;
		goldFactor = 1.5f;
	}

	public void Inspect(EquippableItem item)
	{
		Vector2 value = Cost(item.level); //x-gold, y-shards
		cost.text = "Gold: " + value.x + " Shards: " + value.y;

		if(mode == 1)
			level.text = "Lvl." + item.level + " -> " +"Lvl."+(item.level+1);
	}

	public void Sell(Item itemToSell)
	{
		GameControl.control.gold += goldBase;
		GameControl.control.shards += shardBase;

		GameControl.control.inventoryItems.Remove(itemToSell);
		itemToSell.Destroy();
		InventoryManager.im.inventory.RefreshUI();
		DefaultText();
		DisplayResources();
		InventoryManager.im.StatDisplay();
	}

	public void Upgrade(Item itemToUpgrade)
	{
		int goldCost = (int)(goldBase * (itemToUpgrade.level * goldFactor));
		int shardCost = (int)(shardBase * (itemToUpgrade.level * shardFactor));

		if(GameControl.control.gold - goldCost >= 0 && GameControl.control.shards - shardCost >= 0)
		{
			itemToUpgrade.level += 1;

			GameControl.control.gold -= goldCost;
			GameControl.control.shards -= shardCost;

			Inspect((EquippableItem)itemToUpgrade);
			InventoryManager.im.StatDisplay();
		}
		Inspect((EquippableItem)itemToUpgrade);
		DisplayResources();
	}

	private Vector2 Cost(int level)
	{
		if (mode == 1)
		{
			int goldCost = (int)(goldBase * (level * goldFactor));
			int shardCost = (int)(shardBase * (level * shardFactor));
			return new Vector2(goldCost,shardCost);
		}else if(mode == 2)
		{
			int goldCost = goldBase;
			int shardCost = shardBase;
			return new Vector2(goldCost,shardCost);
		}
		return Vector2.zero;
	}

    public void Choose(int action)
    {
        mode = action;
        Debug.Log("mode " + mode);

		buttons.SetActive(false);

        text.text = @"Left Click to "+ (mode == 1 ? "upgrade" : "sell") +System.Environment.NewLine+"cursor over item to see " + (mode == 1 ? "cost" : "reward");

		DefaultText();
		if(mode == 1)
		{
			level.gameObject.SetActive(true);
			cost.gameObject.SetActive(true);
		}else if(mode == 2)
		{
			cost.gameObject.SetActive(true);
		}
		resources[0].gameObject.SetActive(true);
		resources[1].gameObject.SetActive(true);
		DisplayResources();
    }

	public void DefaultText()
	{
		if(mode == 1)
		{
			level.text = "Lv.? -> Lv.?";
			cost.text = "Cost: ";
		}else if(mode == 2)
		{
			cost.text = "Cost: ";
		}
	}

	private void OnDisable()
	{
		level.gameObject.SetActive(false);
		cost.gameObject.SetActive(false);
		
		resources[0].gameObject.SetActive(false);
		resources[1].gameObject.SetActive(false);

		buttons.SetActive(true);
		mode = 0;
	}

	public void DisplayResources()
	{
		resources[0].ValueText.text = GameControl.control.gold.ToString();
		resources[1].ValueText.text = GameControl.control.shards.ToString();
	}
}
