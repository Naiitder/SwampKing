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
        
        Debug.Log($"QuickSlot {idx} seleccionado");
    }
    
    public void AssignToSelectedSlot(InventorySlot slot)
    {
        if (selectedSlot < 0) return;
        hotbar[selectedSlot] = slot;
        slotUIs[selectedSlot].SetSlot(slot);
        assignMode = false;
        Debug.Log($"Asignado {slot.itemData.name} a QuickSlot {selectedSlot}");
    }
    
    public void HandleSwapSlot()
    {
        if (selectedSlot >= 0 && Input.GetKeyDown(KeyCode.E))
        {
            assignMode = true;
            Debug.Log("Assign mode ON for slot " + selectedSlot);
        }
    }

}
