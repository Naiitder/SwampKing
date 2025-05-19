using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image background;   
    public Image icon;        
    public TextMeshProUGUI quantityText;
    
    public void SetItem(ItemData item, int quantity)
    {
        if (item == null)
        {
            icon.enabled = false;
            quantityText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = item.icon;
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            background.color = Color.black;
        }
    }
}