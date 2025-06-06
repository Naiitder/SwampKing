using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int maximumHealth, currentHealth;
    [SerializeField] private int damage;
    [SerializeField] private String title;
    
    public int MaximumHealth {get{return maximumHealth;} set{maximumHealth = value;}}
    public int CurrentHealth {get{return currentHealth;} set{currentHealth = value;}}
    public int Damage {get{return damage;} set{damage = value;}}
    public String Title {get{return title;} set{title = value;}}
    public int ID {get{return id;} set{id = value;}}
    
   public void LoadStats()
{
    int saveId = GameController.instance.SaveID;

    using (var connection = new SqliteConnection(SQLiteDB.instance.dbName))
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
                SELECT max_health, current_health, damage
                FROM character_stats_state
                WHERE character_id = @charId AND save_id = @saveId;";
            
            command.Parameters.AddWithValue("@charId", id);
            command.Parameters.AddWithValue("@saveId", saveId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    maximumHealth = Convert.ToInt32(reader["max_health"]);
                    damage = Convert.ToInt32(reader["damage"]);

                    currentHealth = reader["current_health"] != DBNull.Value
                        ? Convert.ToInt32(reader["current_health"])
                        : maximumHealth;

                    Debug.Log($"[CharacterStats] Stats personalizados para {id} - Vida: {currentHealth}/{maximumHealth}, Daño: {damage}");
                    return;
                }
            }
        }
        
        using (var fallbackCmd = connection.CreateCommand())
        {
            fallbackCmd.CommandText = @"
                SELECT s.max_health, s.damage
                FROM character c
                JOIN statistics s ON c.statistics = s.id
                WHERE c.id = @charId;";
            
            fallbackCmd.Parameters.AddWithValue("@charId", id);

            using (var reader = fallbackCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    maximumHealth = Convert.ToInt32(reader["max_health"]);
                    damage = Convert.ToInt32(reader["damage"]);
                    currentHealth = maximumHealth;
                    
                }
                else
                {
                    Debug.LogWarning($"[CharacterStats] No se encontró plantilla para character_id {id}");
                }
            }
        }
    }
}
}
