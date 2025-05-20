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
    
    [HideInInspector] public int selectedSlot = -1;
    [HideInInspector] public bool assignMode = false;
    
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
    
    public void DeselectQuickSlot()
    {
        selectedSlot = -1;
        assignMode = false;
        for (int i = 0; i < slotUIs.Length; i++)
            slotUIs[i].SetSelected(false);
    }
    
    public void SelectQuickSlot(int idx)
    {
        assignMode = false;

        selectedSlot = idx;
        for (int i = 0; i < slotCount; i++)
            slotUIs[i].SetSelected(i == idx);
        
        var slot = hotbar[idx];
        if (slot != null)
        {
            
            if (!Inventory.instance.isGameMenuOpen)
                Inventory.instance.HandleGameMenu();

            Inventory.instance.SelectSlot(slot);
        }
        else
        {
            Inventory.instance.detailPanel.Clear();
        }
    }
    
    public void AssignToSelectedSlot(InventorySlot slot)
    {
        if (selectedSlot < 0) return;
        hotbar[selectedSlot] = slot;
        slotUIs[selectedSlot].SetSlot(slot);
        assignMode = false;
    }
    
    public void HandleSwapSlot()
    {
        if (selectedSlot >= 0 && InputController.instance.CheckActions(InputController.InputActionType.Interact))
        {
            InputController.instance.InputBuffer.Dequeue();
            assignMode = true;
        }
    }
    
    public void HandleRemoveSlot()
    {
        if (selectedSlot < 0) return;

        if (InputController.instance.IsDequeuePressed)
        {
            if (InputController.instance.CheckActions(InputController.InputActionType.Attack))
                InputController.instance.InputBuffer.Dequeue();
            
            if (hotbar[selectedSlot] != null)
            {
                hotbar[selectedSlot] = null;
                slotUIs[selectedSlot].SetSlot(null);
            }
        }
    }

}
