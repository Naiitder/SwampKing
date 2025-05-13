using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Mono.Data.Sqlite;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public static GameController instance; 
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject saveGameCanvas;
    [SerializeField] private CameraController CameraController;
    
    [SerializeField] private TextMeshProUGUI coinsText;

    [SerializeField] public int Coins;
    [SerializeField] public int SaveID = -1;

    public bool isGamePaused;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        Application.targetFrameRate = 60; 
        QualitySettings.vSyncCount = 0;
        
        if(pauseCanvas) pauseCanvas.SetActive(false);
        if(gameOverCanvas) gameOverCanvas.SetActive(false);
        if(saveGameCanvas) saveGameCanvas.SetActive(false);

    }

    private void Start()
    {
        SaveID = PlayerPrefs.GetInt("CurrentSaveId", -1);
        if(SaveID > 0) LoadDataFromSave(SaveID);
        PlayerPrefs.SetInt("CurrentSaveId", -1);
        PlayerPrefs.Save();
        
        foreach (CharacterStats stats in FindObjectsByType<CharacterStats>(FindObjectsSortMode.None))
        {
            stats.LoadStats();
        }

        UpdateCoins();
    }

    private void Update()
    {
        if (InputController.instance != null && InputController.instance.IsPausePressed)
        {
            PauseGame();
            InputController.instance.IsPausePressed = false;
        }

        LevelManager.instance?.UpdateProgressBar();
    }

    public void UpdateCoins(int coins = 0)
    {
        Coins += coins;
        if(coinsText != null) coinsText.text = "x" + Coins.ToString();
    }
    
    
    public void SaveCurrentGame(int saveId)
    {
            Vector3 currentPos = InputController.instance.transform.position;
            Quaternion currentRot = InputController.instance.transform.rotation;
            
            SaveGame(saveId, currentPos, currentRot , Coins);
    }
    

    public void PauseGame()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            SetPause();
        }
    }

    public void SetPause()
    {
        pauseCanvas.SetActive(true);
        isGamePaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseCanvas.SetActive(false);
        DeActiveSaveGameCanvas();
        isGamePaused = false;
        Time.timeScale = 1;
    }


    public void NewGame()
    {
        int newSaveId = SQLiteDB.instance.CreateNewSaveSlot();

        if (newSaveId != -1)
        {
            SQLiteDB.instance.InsertInitialGameData(newSaveId);
            
            PlayerPrefs.SetInt("CurrentSaveId", newSaveId);
            PlayerPrefs.Save();
            
            LevelManager.instance.LoadScene("SampleScene");
        }
        else
        {
            //TODO mostrar en UI que no se puede
            Debug.Log("No puedes crear más partidas guardadas.");
        }

    }
    
    public void LoadDataFromSave(int saveId)
{
    using (var connection = new SqliteConnection(SQLiteDB.instance.dbName))
    {
        connection.Open();

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT name, position, rotation, coins FROM player WHERE save_id = @saveId LIMIT 1;";
            cmd.Parameters.AddWithValue("@saveId", saveId);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = reader["name"].ToString();
                    string position = reader["position"].ToString();
                    string rotation = reader["rotation"].ToString();
                    int coins = Convert.ToInt32(reader["coins"]);
                    
                    string[] parts = position.Split(',');
                    Vector3 pos = new Vector3(
                        float.Parse(parts[0], CultureInfo.InvariantCulture),
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture)
                    );
                    
                    parts = rotation.Split(',');
                    Quaternion rot = new Quaternion(
                        float.Parse(parts[0], CultureInfo.InvariantCulture),
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    );
                    
                    Coins = coins; 
                    StartCoroutine(SetPlayerPositionNextFrame(pos, rot));

                    // TODO Establecer stats... etc.
                    Debug.Log($"Loaded player {name} at {position} with {coins} coins.");
                }
            }
        }
        
        Inventory.instance.LoadInventoryFromDatabase(saveId);

        /*
        // --- QUESTS ---
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT quest_id, state, progress FROM quest_state WHERE save_id = @saveId;";
            cmd.Parameters.AddWithValue("@saveId", saveId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int questId = Convert.ToInt32(reader["quest_id"]);
                    string state = reader["state"].ToString();
                    int progress = Convert.ToInt32(reader["progress"]);

                    QuestManager.instance.SetQuestState(questId, state, progress);
                    Debug.Log($"Quest {questId} - {state} ({progress}%)");
                }
            }
        }*/

        /*// --- CHARACTERS (is_alive) ---
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT character_id, is_alive FROM character_state WHERE save_id = @saveId;";
            cmd.Parameters.AddWithValue("@saveId", saveId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int charId = Convert.ToInt32(reader["character_id"]);
                    bool isAlive = Convert.ToInt32(reader["is_alive"]) == 1;

                    CharacterManager.instance.SetCharacterAlive(charId, isAlive);
                }
            }
        }

        // --- EVENTS (opcional) ---
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT id, triggered FROM event_state WHERE save_id = @saveId;";
            cmd.Parameters.AddWithValue("@saveId", saveId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string eventId = reader["id"].ToString();
                    bool triggered = Convert.ToInt32(reader["triggered"]) == 1;

                    EventManager.instance.SetEventTriggered(eventId, triggered);
                }
            }
        }*/

        connection.Close();
    }
}
    public void SaveGame(int saveId, Vector3 position, Quaternion rotation, int coins)
        {
            using (var connection = new SqliteConnection(SQLiteDB.instance.dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    string posString = $"{position.x.ToString(CultureInfo.InvariantCulture)}," +
                                       $"{position.y.ToString(CultureInfo.InvariantCulture)}," +
                                       $"{position.z.ToString(CultureInfo.InvariantCulture)}";

                    string rotString = $"{rotation.x.ToString(CultureInfo.InvariantCulture)}," +
                                       $"{rotation.y.ToString(CultureInfo.InvariantCulture)}," +
                                       $"{rotation.z.ToString(CultureInfo.InvariantCulture)}," +
                                       $"{rotation.w.ToString(CultureInfo.InvariantCulture)}";
                    
                    command.CommandText = @"
                UPDATE player 
                SET position = @position, rotation = @rotation, coins = @coins
                WHERE save_id = @saveId;
                
                UPDATE save_slot
                SET play_time = TIME('now') 
                WHERE id = @saveId;
            ";
                    command.Parameters.AddWithValue("@position", posString);
                    command.Parameters.AddWithValue("@rotation", rotString);
                    command.Parameters.AddWithValue("@coins", coins);
                    command.Parameters.AddWithValue("@saveId", saveId);

                    command.ExecuteNonQuery();
                }
                foreach (var stats in FindObjectsByType<CharacterStats>(FindObjectsSortMode.None))
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = @"
            UPDATE character_stats_state
            SET current_health = @currentHealth,
                max_health = @maxHealth,
                damage = @damage
            WHERE character_id = @charId AND save_id = @saveId;";

                        cmd.Parameters.AddWithValue("@currentHealth", stats.CurrentHealth);
                        cmd.Parameters.AddWithValue("@maxHealth", stats.MaximumHealth);
                        cmd.Parameters.AddWithValue("@damage", stats.Damage);
                        cmd.Parameters.AddWithValue("@charId", stats.ID);
                        cmd.Parameters.AddWithValue("@saveId", saveId);

                        cmd.ExecuteNonQuery();
                    }
                }
                
                using (var deleteCmd = connection.CreateCommand())
                {
                    deleteCmd.CommandText = "DELETE FROM inventory WHERE save_id = @saveId;";
                    deleteCmd.Parameters.AddWithValue("@saveId", saveId);
                    deleteCmd.ExecuteNonQuery();
                }

                foreach (var slot in Inventory.instance.items)
                {
                    using (var insertCmd = connection.CreateCommand())
                    {
                        insertCmd.CommandText = @"
        INSERT INTO inventory (save_id, id_item, quantity)
        VALUES (@saveId, @itemId, @quantity);";

                        insertCmd.Parameters.AddWithValue("@saveId", saveId);
                        insertCmd.Parameters.AddWithValue("@itemId", slot.itemData.id);
                        insertCmd.Parameters.AddWithValue("@quantity", slot.quantity);

                        insertCmd.ExecuteNonQuery();
                    }
                }
                
                connection.Close();
            }

            Debug.Log($"✅ Partida {saveId} guardada correctamente.");
        }

    private IEnumerator SetPlayerPositionNextFrame(Vector3 pos, Quaternion rot)
    {
        yield return null; 
        InputController.instance.transform.position = pos;
        InputController.instance.transform.rotation = rot;
    }

    public void ActiveSaveGameCanvas()
    {
        saveGameCanvas.SetActive(true);
    }
    public void DeActiveSaveGameCanvas()
    {
        saveGameCanvas.SetActive(false);
    }
    

}
