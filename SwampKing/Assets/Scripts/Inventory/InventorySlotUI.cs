using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image background;
    public Image icon;
    public TextMeshProUGUI quantityText;

    [HideInInspector] public InventorySlot SlotData { get; private set; }
    
    public void SetData(InventorySlot slot)
    {
        SlotData = slot;
        if (slot == null)
        {
            icon.enabled      = false;
            quantityText.text = "";
        }
        else
        {
            icon.enabled      = true;
            icon.sprite       = slot.itemData.icon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
            background.color = Color.black;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (QuickSlotManager.instance.assignMode)
        {
            if (SlotData != null)
                QuickSlotManager.instance.AssignToSelectedSlot(SlotData);
            return;
        }
        
        QuickSlotManager.instance.DeselectQuickSlot();
        Inventory.instance.SelectSlot(SlotData);
    }
}