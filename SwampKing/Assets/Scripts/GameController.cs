using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] private GameObject _pauseCanvas;

    [SerializeField] public int Coins;
    [SerializeField] public int SaveID = -1;

    public bool isGamePaused;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        Application.targetFrameRate = 60; 
        QualitySettings.vSyncCount = 0;
        if(_pauseCanvas) _pauseCanvas.SetActive(false);
        
        if(SaveID > 0) SQLiteDB.instance.LoadDataFromSave(SaveID);
    }

    private void Update()
    {
        if (InputController.instance != null && InputController.instance.IsPausePressed)
        {
            PauseGame();
            InputController.instance.IsPausePressed = false; // Resetear el input tras usarlo
        }

        LevelManager.instance?.UpdateProgressBar();
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
        _pauseCanvas.SetActive(true);
        isGamePaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        _pauseCanvas.SetActive(false);
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
    
    
}
