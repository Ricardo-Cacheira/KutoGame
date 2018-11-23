using System;
using UnityEngine;

public enum EquipmentType {
	Helmet,
	Chest,
	Gloves,
	Boots
}

[CreateAssetMenu]
[Serializable]
public class EquippableItem : Item {


	// public int strenghtBonus;
	// public int vitalityBonus;
	public int level;

	[Space] // Visible in unity editor
	
	public int skillID;

	[Space] // Visible in unity editor
	
	public EquipmentType equipmentType;

	public override Item GetCopy()
	{
		return Instantiate(this);
	}

	public override void Destroy()
	{
		Destroy(this);
	}

}
