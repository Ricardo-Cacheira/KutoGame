
public class EquipmentSlots : ItemSlot {

	public EquipmentType equipmentType;

	protected override void OnValidate()
	{
		base.OnValidate();

		gameObject.name = equipmentType.ToString() + " Slot";
	}
}
