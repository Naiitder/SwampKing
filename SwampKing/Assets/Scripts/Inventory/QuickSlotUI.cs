using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image selectedHighlight;  
    [HideInInspector] public int index;
    
    public void SetSlot(InventorySlot slot)
    {
        if (slot == null)
        {
            icon.enabled = false;
        }
        else
        {
            icon.enabled = true;
            icon.sprite  = slot.itemData.icon;
        }
    }
    
    public void SetSelected(bool sel)
    {
        if (sel)
        {
            if (selectedHighlight != null)
                selectedHighlight.color = Color.blue;
        }
        else
        {
            if (selectedHighlight != null)
                selectedHighlight.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        QuickSlotManager.instance.SelectQuickSlot(index);
    }
}
