using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour {

	[SerializeField] Text itemName;
	[SerializeField] Text itemSlot;
	[SerializeField] Text itemSkill;

	void Awake()
	{
		HideToolTip();
	}

	public void ShowToolTip(EquippableItem item)
	{
		itemName.text = item.name;
		itemSlot.text = item.equipmentType.ToString();
		itemSkill.text = "SkillId: "+ item.skillID.ToString()+ " at Level "+item.level+"\nIt does stuff";

		gameObject.SetActive(true);
	}

	public void HideToolTip()
	{
		gameObject.SetActive(false);
	}
}
