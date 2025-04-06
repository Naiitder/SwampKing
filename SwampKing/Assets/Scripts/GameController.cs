using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _pauseCanvas;

    public bool isGamePaused;

    void Start()
    {
        Application.targetFrameRate = 60; 
        QualitySettings.vSyncCount = 0;
        _pauseCanvas.SetActive(false);
    }

    private void Update()
    {
        PauseGame();
        LevelManager.instance.UpdateProgressBar();
    }


    void PauseGame()
    {
        if (InputController.instance != null)
        {
            if (InputController.instance.IsPausePressed)
            {
                _pauseCanvas.SetActive(true);
                isGamePaused = true;
                Time.timeScale = 0;
            }
            else
            {
                _pauseCanvas.SetActive(false);
                isGamePaused = false;
                Time.timeScale = 1;

            }
        }
    }
}
