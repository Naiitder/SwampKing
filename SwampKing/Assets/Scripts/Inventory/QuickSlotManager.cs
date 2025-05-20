using UnityEngine;
using System.Linq;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;
    [Header("UI")]
    public RectTransform quickSlotPanel;  
    public QuickSlotUI quickSlotPrefab;    
    public int slotCount = 5;
    
    InventorySlot[] hotbar;   
    QuickSlotUI[] slotUIs;
    int selectedHotbar = -1;
    
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        hotbar = new InventorySlot[slotCount];
        slotUIs = new QuickSlotUI[slotCount];
        
        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(quickSlotPrefab, quickSlotPanel);
            ui.index = i;
            ui.SetSlot(null);
            ui.SetSelected(false);
            slotUIs[i] = ui;
        }
    }
    
    public void OnQuickSlotClicked(int idx)
    {
        if (selectedHotbar >= 0)
        {
            DeselectHotbar();
            return;
        }
        
        var slot = hotbar[idx];
        if (slot != null)
        {
            Inventory.instance.SelectSlot(slot);  
            SelectHotbar(idx);
        }
        else
        {
            SelectHotbar(idx);
            Inventory.instance.StartAssignMode(idx);
        }
    }
    
    void SelectHotbar(int idx)
    {
        selectedHotbar = idx;
        for (int i = 0; i < slotCount; i++)
            slotUIs[i].SetSelected(i == idx);
    }
    
    void DeselectHotbar()
    {
        selectedHotbar = -1;
        for (int i = 0; i < slotCount; i++)
            slotUIs[i].SetSelected(false);
        Inventory.instance.EndAssignMode();
    }
    
    public void OnInventorySlotClicked(InventorySlot slot)
    {
        if (selectedHotbar >= 0)
        {
            if (hotbar[selectedHotbar] == slot)
                hotbar[selectedHotbar] = null;
            else
                hotbar[selectedHotbar] = slot;
            
            slotUIs[selectedHotbar].SetSlot(hotbar[selectedHotbar]);
            DeselectHotbar();
        }
        else
        {
            Inventory.instance.SelectSlot(slot);
        }
    }
    
    public bool IsAssigned(InventorySlot slot)
        => hotbar.Contains(slot);
    
    public void ToggleAssignFromDetail(InventorySlot slot)
    {
        int idx = System.Array.IndexOf(hotbar, slot);
        if (idx >= 0)
        {
            hotbar[idx] = null;
            slotUIs[idx].SetSlot(null);
        }
        else
        {
            int free = System.Array.IndexOf(hotbar, null);
            if (free >= 0)
            {
                hotbar[free] = slot;
                slotUIs[free].SetSlot(slot);
            }
        }
    }

}
