using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> items = new List<InventorySlot>();
    public int maxSlots = 20;
    public static Inventory instance;
    
    [SerializeField] private GameObject gameMenuCanvas;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlotUI slotPrefab;
    
    public ItemDetailPanel detailPanel;
    
    public bool isGameMenuOpen;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        if(gameMenuCanvas) gameMenuCanvas.SetActive(false);
    }
    public void AddItem(ItemData itemData, int amount)
    {
        InventorySlot slot = items.Find(x => x.itemData.id == itemData.id);

        if (slot != null)
        {
            slot.quantity += amount;
        }
        else if (items.Count < maxSlots)
        {
            items.Add(new InventorySlot(itemData, amount));
        }
        else
        {
            Debug.LogWarning("Inventario lleno. No se puede añadir más items.");
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
    
    public void LoadInventoryFromDatabase(int saveId)
    {
        items.Clear();

        using (var connection = new Mono.Data.Sqlite.SqliteConnection(SQLiteDB.instance.dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id_item, quantity FROM inventory WHERE save_id = @saveId;";
                command.Parameters.AddWithValue("@saveId", saveId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int itemId = reader.GetInt32(0);
                        int quantity = reader.GetInt32(1);
                        ItemData item = ItemDatabase.instance.GetItemById(itemId);
                        if (item != null)
                            AddItem(item, quantity);
                    }
                }
            }
        }
    }
    
    public void HandleGameMenu()
    {
        if (isGameMenuOpen)
        {
            gameMenuCanvas.SetActive(false);
            isGameMenuOpen = false;
        }
        else
        {
            RefreshUI();
            gameMenuCanvas.SetActive(true);
            isGameMenuOpen = true;
        }
        detailPanel.Clear();
        
    }
    
    public void RefreshUI()
    {
        foreach (Transform t in inventoryPanel.transform)
            Destroy(t.gameObject);
        
        for (int i = 0; i < maxSlots; i++)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, inventoryPanel.transform);
            if (i < items.Count)
            {
                slotUI.SetData(items[i]);
            }
            else
            {
                slotUI.SetData(null);
            }
        }
    }
    
    public void SelectSlot(InventorySlot slot)
    {
        if (!isGameMenuOpen) return;
        if (slot != null)
            detailPanel.Show(slot);
        else
            detailPanel.Clear();
    }
}
