using System;
using UnityEngine;

public class CanvasReferences : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject saveGameCanvas;

    private void Start()
    {
        GameController.instance.SetSceneUI(pauseCanvas, gameOverCanvas,saveGameCanvas);
        pauseCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        saveGameCanvas.SetActive(false);
    }
    
    public void ActiveSaveOptions()
    {
        saveGameCanvas.SetActive(true);
    }

    public void DeactiveSaveOptions()
    {
        saveGameCanvas.SetActive(false);
    }
}
