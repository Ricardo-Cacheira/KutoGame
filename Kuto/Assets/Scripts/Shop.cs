// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Shop : MonoBehaviour {

// 	public float shardFactor;
// 	public float goldFactor;
// 	public float shardBase;
// 	public float goldBase;

// 	void Start()
// 	{
// 		shardBase = 5f;
// 		goldBase = 150f;
// 		shardFactor = 1f;
// 		goldFactor = 1.5f;
// 	}

// 	public void Sell(Item itemToSell)
// 	{
// 		GameControl.control.gold += goldBase;
// 		GameControl.control.shard += shardBase;
// 		// GameControl.control.gold += goldBase * (itemToSell.level * goldFactor);
// 		// GameControl.control.shard += shardBase * (itemToSell.level * shardFactor);
// 	}

// 	public void Upgrade(Item itemToSell)
// 	{
// 		itemToSell.level += 1;

// 		int goldCost = (int)goldBase * (itemToSell.level * goldFactor);
// 		int shardCost = (int)shardBase * (itemToSell.level * shardFactor);

// 		GameControl.control.gold -= goldCost;
// 		GameControl.control.shard -= shardCost;
// 	}
// }
