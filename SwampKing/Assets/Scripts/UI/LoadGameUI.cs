using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameUI : MonoBehaviour
{
    [SerializeField] private GameObject slotButtonPrefab;
    [SerializeField] private Transform containerTransform;
    
    void Start()
    {
        foreach (var slot in SQLiteDB.instance.GetAllSaveSlots())
        {
            GameObject newButton = Instantiate(slotButtonPrefab, containerTransform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text =
                $"Partida {slot.id} - {slot.location} - Tiempo: {slot.playTime} - {slot.createdAt}";

            newButton.GetComponent<Button>().onClick.AddListener(() => {
                LoadSave(slot.id);
            });
        }
    }
    
    public void LoadSave(int saveId)
    {
        PlayerPrefs.SetInt("CurrentSaveId", saveId);
        PlayerPrefs.Save();
        
        LevelManager.instance.LoadScene("SampleScene");
    }

}
