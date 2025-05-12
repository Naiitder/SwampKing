using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> items = new List<InventorySlot>();

    public void AddItem(ItemData itemData, int amount)
    {
        InventorySlot slot = items.Find(x => x.itemData.id == itemData.id);

        if (slot != null)
        {
            slot.quantity += amount;
        }
        else
        {
            items.Add(new InventorySlot(itemData, amount));
        }
    }

    public void RemoveItem(ItemData itemData, int amount)
    {
        InventorySlot slot = items.Find(x => x.itemData.id == itemData.id);

        if (slot != null)
        {
            slot.quantity -= amount;
            if (slot.quantity <= 0)
                items.Remove(slot);
        }
    }
    
}
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public InventorySlot(ItemData data, int amount)
    {
        itemData = data;
        quantity = amount;
    }
}