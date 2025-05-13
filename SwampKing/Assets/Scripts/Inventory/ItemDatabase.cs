using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public static ItemDatabase instance;
    private string dbName = "URI=file:DataBase.db";
    
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        LoadItemsFromDatabase();
    }
     public void LoadItemsFromDatabase()
    {
        items.Clear();
        using (var connection = new Mono.Data.Sqlite.SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM item;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemData item = new ItemData
                        {
                            id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            description = reader.GetString(2),
                            price = reader.GetInt32(3),
                            type = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };

                        // Si es arma, buscar daño
                        if (item.type == "weapon")
                        {
                            item.damage = GetWeaponDamage(item.id, connection);
                        }
                        else if (item.type == "armor")
                        {
                            item.armor = GetArmorValue(item.id, connection);
                        }

                        items.Add(item);
                    }
                }
            }
        }
    }

    private int GetWeaponDamage(int itemId, Mono.Data.Sqlite.SqliteConnection connection)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT damage FROM weapon WHERE id_item = @id";
            cmd.Parameters.AddWithValue("@id", itemId);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    private int GetArmorValue(int itemId, Mono.Data.Sqlite.SqliteConnection connection)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT armor FROM armor WHERE id_item = @id";
            cmd.Parameters.AddWithValue("@id", itemId);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    public ItemData GetItemById(int id)
    {
        return items.Find(x => x.id == id);
    }
}
