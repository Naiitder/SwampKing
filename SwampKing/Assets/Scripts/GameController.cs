using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] private GameObject _pauseCanvas;

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
        if(_pauseCanvas) _pauseCanvas.SetActive(false);
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
    
    
    
}
