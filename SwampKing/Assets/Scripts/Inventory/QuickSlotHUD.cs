using System;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotHUD : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;

    private void Awake()
    {
        iconImage.enabled = false;
    }

    public void UpdateSelectedItem(InventorySlot slot)
    {
        if (slot != null)
        {
            iconImage.sprite = slot.itemData.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
    }
}
