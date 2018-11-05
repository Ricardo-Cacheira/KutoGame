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

	[Space] // Visible in unity editor
	
	public int skillID;

	[Space] // Visible in unity editor
	
	public EquipmentType equipmentType;

}
