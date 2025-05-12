using System;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject loadGameScreen;

    private void Awake()
    {
        loadGameScreen.SetActive(false);
    }

    public void ActiveLoadMenu()
    {
        loadGameScreen.SetActive(true);
    }
    
    public void DeactiveLoadMenu()
    {
        loadGameScreen.SetActive(false);
    }
}
