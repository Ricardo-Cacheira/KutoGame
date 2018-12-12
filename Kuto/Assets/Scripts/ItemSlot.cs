using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{

	//allows the variable to remain private and still show up in the editor, prenventing accidental changes to it in the code
	[SerializeField] Image image;
	[SerializeField] ItemToolTip tooltip;

	public event Action<Item> OnRightClickEvent;
	public event Action<Item> OnLeftClickEvent;
	public event Action<Item> OnMobileClickedEvent;

	private Item _item;
	public Item Item{
		get{ return _item; }
		set{
			_item = value;
			if(_item == null){
				image.enabled = false;
			}else{
				image.sprite = _item.icon;
				image.enabled = true;
			}
		}
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(Application.platform);

		if(eventData != null && eventData.button == PointerEventData.InputButton.Right)
		{
			if(Item != null && OnRightClickEvent != null)
			{
				OnRightClickEvent(Item);
				tooltip.HideToolTip();
			}
            Debug.Log("Right Touch");
		}else if(eventData != null && eventData.button == PointerEventData.InputButton.Left)
		{
            Debug.Log("Left Touch");
			if(Item != null && OnLeftClickEvent != null)
			{
				OnLeftClickEvent(Item);
				tooltip.HideToolTip();
			}
			
		} else if(eventData != null && eventData.button != PointerEventData.InputButton.Middle){ //
			OnMobileClickedEvent(Item);
			tooltip.HideToolTip();
            Debug.Log("Mobile Touch");
		}
    }

    protected virtual void OnValidate()
	{
		//Only called on editor and triggers when script is loaded or change one of its values on the editor
		//used this to fill the image variable for each slot
		if(image==null)
		image = GetComponent<Image>();

		if(tooltip==null)
		tooltip = FindObjectOfType<ItemToolTip>();
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
		if(Item is EquippableItem)
		{
			switch(Shop.instance.mode)
			{
				case 0: 
					tooltip.ShowToolTip((EquippableItem)Item);
					break;
				case 1:
					Shop.instance.Inspect((EquippableItem)Item);
					break;
				case 2:
					Shop.instance.Inspect((EquippableItem)Item);
					break;
				default:
					tooltip.ShowToolTip((EquippableItem)Item);
					break;
			}
			tooltip.ShowToolTip((EquippableItem)Item);
		}
        	
    }

    public void OnPointerExit(PointerEventData eventData)
    {
		if(Item is EquippableItem)
		{
			switch(Shop.instance.mode)
			{
				case 0: 
					tooltip.HideToolTip();
					break;
				case 1:
					Shop.instance.DefaultText();
					break;
				case 2:
					Shop.instance.DefaultText();
					break;
				default:
					tooltip.HideToolTip();
					break;
			}
		}
		tooltip.HideToolTip();
    }
}
