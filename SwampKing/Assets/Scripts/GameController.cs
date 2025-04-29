using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
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
    
    public void SetSceneUI(GameObject pause, GameObject gameOver, GameObject save)
    {
        pauseCanvas = pause;
        gameOverCanvas = gameOver;
        saveGameCanvas = save;

        pauseCanvas.SetActive(false);
        saveGameCanvas.SetActive(false);
    }
    
    public void SaveCurrentGame(int saveId)
    {
            Vector3 currentPos = InputController.instance.transform.position;
            Quaternion currentRot = InputController.instance.transform.rotation;
            

            float currentCameraRot = CameraController.transform.rotation.eulerAngles.y;
            
            SQLiteDB.instance.SaveGame(saveId, currentPos, currentRot, currentCameraRot , Coins);
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
        isGamePaused = false;
        Time.timeScale = 1;
    }


    public void NewGame()
    {
        int newSaveId = SQLiteDB.instance.CreateNewSaveSlot();

        if (newSaveId != -1)
        {
            SQLiteDB.instance.InsertInitialGameData(newSaveId);
            Debug.Log($"Nueva partida creada con ID: {newSaveId}");
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

        // --- PLAYER ---
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT name, position, rotation, camera_rotation, coins, statistics FROM player WHERE save_id = @saveId LIMIT 1;";
            cmd.Parameters.AddWithValue("@saveId", saveId);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = reader["name"].ToString();
                    string position = reader["position"].ToString();
                    string rotation = reader["rotation"].ToString();
                    int coins = Convert.ToInt32(reader["coins"]);
                    int statsId = Convert.ToInt32(reader["statistics"]);
                    float cameraRot = Convert.ToSingle(reader["camera_rotation"]);
                    
                    string[] parts = position.Split(',');
                    Vector3 pos = new Vector3(
                        float.Parse(parts[0]),
                        float.Parse(parts[1]),
                        float.Parse(parts[2])
                    );
                    
                    parts = rotation.Split(',');
                    Quaternion rot = new Quaternion(
                        float.Parse(parts[0]),
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])
                    );
                    
                    Coins = coins; 
                    StartCoroutine(SetPlayerPositionNextFrame(pos, rot,
                        cameraRot));

                    // TODO Establecer stats... etc.
                    Debug.Log($"Loaded player {name} at {position} with {coins} coins.");
                }
            }
        }

        /*// --- INVENTORY ---
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT id_item, quantity FROM inventory WHERE save_id = @saveId;";
            cmd.Parameters.AddWithValue("@saveId", saveId);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int itemId = Convert.ToInt32(reader["id_item"]);
                    int quantity = Convert.ToInt32(reader["quantity"]);

                    // Añadir item al inventario del jugador, ejemplo:
                    InventoryManager.instance.AddItem(itemId, quantity);

                    Debug.Log($"Item {itemId} x{quantity}");
                }
            }
        }

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
    private IEnumerator SetPlayerPositionNextFrame(Vector3 pos, Quaternion rot, float cameraRotation)
    {
        yield return null; 
        InputController.instance.transform.position = pos;
        InputController.instance.transform.rotation = rot;
        CameraController.lookAngle = cameraRotation;
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
