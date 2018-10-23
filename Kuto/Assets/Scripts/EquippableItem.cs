using UnityEngine;

public enum EquipmentType {
	Helmet,
	Chest,
	Gloves,
	Boots
}

[CreateAssetMenu]
public class EquippableItem : Item {

	public int strenghtBonus;
	public int vitalityBonus;

	[Space] // Visible in unity editor
	
	// public Skill ability;

	[Space] // Visible in unity editor
	
	public EquipmentType equipmentType;

}
