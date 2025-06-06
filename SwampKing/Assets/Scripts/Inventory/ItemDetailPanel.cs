using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    [Header("UI References")]
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDesc;
    public GameObject detailPanel;
    
    InventorySlot currentSlot;
    
    
    void Awake()
    {
        Clear();
    }
    
    public void Show(InventorySlot slot)
    {
        currentSlot = slot;
        detailPanel.SetActive(true);
        
        detailIcon.sprite = slot.itemData.icon;
        detailIcon.enabled = true;
        detailName.text   = slot.itemData.name;
        detailDesc.text   = slot.itemData.description;
        
    }
    
    public void Clear()
    {
        currentSlot = null;
        detailIcon.enabled = false;
        detailName.text   = "";
        detailDesc.text   = "";
        detailPanel.SetActive(false);
    }
}
