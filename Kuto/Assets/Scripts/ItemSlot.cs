using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour , IPointerClickHandler{ //to detect Clicks

	//allows the variable to remain private and still show up in the editor, prenventing accidental changes to it in the code
	[SerializeField] Image image;

	public event Action<Item> OnRightClickEvent;

	public Item _item;
	public Item Item{
		get{ return _item; }
		set{
			_item = value;
			if(_item == null)
			image.enabled = false;
			else
			{
				image.sprite = _item.icon;
				image.enabled = true;
			}
		}
	}
	


	protected virtual void OnValidate()
	{
		//Only called on editor and triggers when script is loaded or change one of its values on the editor
		//used this to fill the image variable for each slot
		if(image==null)
		image = GetComponent<Image>();

	}

    public void OnPointerClick(PointerEventData eventData)
    {
		if(eventData != null && eventData.button == PointerEventData.InputButton.Right)
		{
			if(Item != null && OnRightClickEvent != null)
				OnRightClickEvent(Item);
		}
    }
}
