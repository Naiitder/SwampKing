using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;
    
    [SerializeField] QuickSlotHUD quickSlotHUD;
    
    [Header("UI")]
    public RectTransform quickSlotPanel;  
    public QuickSlotUI quickSlotPrefab;    
    public int slotCount = 5;
    
    InventorySlot[] hotbar;   
    QuickSlotUI[] slotUIs;
    
    [HideInInspector] public int selectedSlot = -1;
    [HideInInspector] public bool assignMode = false;
    
    public int indexQuickItem = 0;
    
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
    
    public void HandleUseInput()
    {
        if (indexQuickItem >= 0 && indexQuickItem < slotCount)
        {
            Debug.Log("primero");
            InventorySlot slot = hotbar[indexQuickItem];
            if (slot != null && slot.itemData.isUsable)
            {
                Debug.Log("segundo");
                Inventory.instance.UseItem(slot);
                quickSlotHUD.UpdateSelectedItem(slot); 
            }
        }
    }
    
    public void HandleCycleInput()
    {
        var assigned = GetAssignedIndices();
        if (assigned.Count < 2) return;
        
        int dir = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow)) dir = +1;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = -1;
        if (dir == 0) return;
        
        int pos = assigned.IndexOf(indexQuickItem);
        if (pos < 0) pos = 0;

        int nextPos = (pos + dir + assigned.Count) % assigned.Count;
        int nextSlot = assigned[nextPos];
        
        CycleHUDSlot(nextSlot);
    }
    
    private List<int> GetAssignedIndices()
    {
        var list = new List<int>();
        for (int i = 0; i < hotbar.Length; i++)
            if (hotbar[i] != null) list.Add(i);
        return list;
    }
    
    public void DeselectQuickSlot()
    {
        selectedSlot = -1;
        assignMode = false;
        for (int i = 0; i < slotUIs.Length; i++)
            slotUIs[i].SetSelected(false);
        if (GetAssignedIndices().Count == 0)
        {
            quickSlotHUD.UpdateSelectedItem(null);
        }
        Inventory.instance.detailPanel.Clear();
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
        
        if (GetAssignedIndices().Count == 1)
        {
            CycleHUDSlot(selectedSlot);
        }
    }
    
    public void HandleSwapSlot()
    {
        if (selectedSlot >= 0 && InputController.instance.CheckActions(InputController.InputActionType.Interact))
        {
            InputController.instance.InputBuffer.Dequeue();
            assignMode = true;
        }
    }
    
    void CycleHUDSlot(int slotIdx)
    {
        indexQuickItem = slotIdx;
        
        quickSlotHUD.UpdateSelectedItem(hotbar[slotIdx]);
    }
    
    public void HandleRemoveSlot()
    {
        if (selectedSlot < 0) return;

        if (InputController.instance.IsDequeuePressed)
        {
            if (InputController.instance.CheckActions(InputController.InputActionType.Attack))
                InputController.instance.InputBuffer.Dequeue();
            
           
            bool removedHud = (selectedSlot == indexQuickItem);
            hotbar[selectedSlot] = null;
            slotUIs[selectedSlot].SetSlot(null);

            if (removedHud)
            {
                var assigned = GetAssignedIndices();
                if (assigned.Count > 0)
                {
                    int pos = assigned.IndexOf(indexQuickItem);
                    if (pos < 0) pos = 0;
                    CycleHUDSlot(assigned[pos]);
                }
                else
                {
                    indexQuickItem = -1;
                    quickSlotHUD.UpdateSelectedItem(null);
                    for (int i = 0; i < slotCount; i++)
                        slotUIs[i].SetSelected(false);
                }
            };
            
            if (hotbar[selectedSlot] != null)
            {
                hotbar[selectedSlot] = null;
                slotUIs[selectedSlot].SetSlot(null);
                quickSlotHUD.UpdateSelectedItem(null);
                
            }   
        }
    }

}
