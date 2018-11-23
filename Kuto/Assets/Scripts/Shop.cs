using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

	public float shardFactor;
	public float goldFactor;
	public int shardBase;
	public int goldBase;

    private Text text;
    [SerializeField] GameObject buttons;

    public int mode; //0- none //1-upgrade //2-sell

	void Start()
	{
        text = transform.GetChild(0).GetComponent<Text>();
        text.text = @"What can I"+System.Environment.NewLine+" help you with?";
		mode = 0;
		shardBase = 5;
		goldBase = 150;
		shardFactor = 1f;
		goldFactor = 1.5f;
	}

	public void Sell(Item itemToSell)
	{
		GameControl.control.gold += goldBase;
		GameControl.control.shards += shardBase;
		// GameControl.control.gold += goldBase * (itemToSell.level * goldFactor);
		// GameControl.control.shard += shardBase * (itemToSell.level * shardFactor);
	}

	public void Upgrade(Item itemToSell)
	{
		itemToSell.level += 1;

		int goldCost = (int)(goldBase * (itemToSell.level * goldFactor));
		int shardCost = (int)(shardBase * (itemToSell.level * shardFactor));

		GameControl.control.gold -= goldCost;
		GameControl.control.shards -= shardCost;
	}

    public void Choose(int action)
    {
        mode = action;
        Debug.Log("mode " + mode);

		buttons.SetActive(false);

        text.text = @"Left Click to "+ (mode == 1 ? "upgrade" : "sell") +System.Environment.NewLine+"cursor over item to see " + (mode == 1 ? "cost" : "reward");
    }
}
