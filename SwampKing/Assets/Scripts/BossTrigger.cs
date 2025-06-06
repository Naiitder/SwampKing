using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossTrigger : MonoBehaviour
{
    [Header("CharacterStats")] 
    public CharacterStats CharacterStats;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider easeHealthSlider;

    [SerializeField] public TextMeshProUGUI bossTitle;

    private GameObject[] fogWalls;

    [SerializeField] private AudioClip bossSong;


    private void Awake()
    {
        if (fogWalls != null)
        {
            foreach (GameObject fogWall in fogWalls)
            {
                fogWall.SetActive(false);
            }
        }
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) TriggerBoss();
    }

    private void TriggerBoss()
    {
        if (fogWalls != null)
        {
            foreach (GameObject fogWall in fogWalls)
            {
                fogWall.SetActive(true);
            }
        }
        GameController.instance.ActivateBossCanvas();
        
        healthSlider.maxValue = CharacterStats.MaximumHealth;
        healthSlider.value = CharacterStats.CurrentHealth;
        
        easeHealthSlider.maxValue = CharacterStats.MaximumHealth;
        easeHealthSlider.value = CharacterStats.CurrentHealth;
        
        bossTitle.text = CharacterStats.Title;
        
        GameController.instance.ChangeSong(bossSong);
        
        gameObject.SetActive(false);
    }
    
    
}
