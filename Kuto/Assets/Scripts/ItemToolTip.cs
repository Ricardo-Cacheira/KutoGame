using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour {

	[SerializeField] Text itemName;
	[SerializeField] Text itemSlot;
	[SerializeField] Text itemSkill;

	private GameControl c;

	void Awake()
	{
		HideToolTip();
		c = GameControl.control;
	}

	public void ShowToolTip(EquippableItem item)
	{
		itemName.text = item.name.Substring(0,item.name.Length-7);
		itemSlot.text = item.equipmentType.ToString();
		// itemSkill.text = "SkillId: "+ item.skillID.ToString()+ " at Level "+item.level+"\nIt does stuff";
		itemSkill.text = "<color=#23b2ff>Lvl. "+ item.level + "</color> " + c.abilities[item.skillID].name +"\n"+ c.abilities[item.skillID].description;

		gameObject.SetActive(true);
	}

	public void HideToolTip()
	{
		gameObject.SetActive(false);
	}
}
