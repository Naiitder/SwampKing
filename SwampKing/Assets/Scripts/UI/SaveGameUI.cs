using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameUI : MonoBehaviour
{
    [SerializeField] private GameObject slotButtonPrefab;
    [SerializeField] private GameObject addNewSaveButtonPrefab;
    [SerializeField] private Transform containerTransform;

    void Start()
    {
        LoadSlotsUI();
    }

    void LoadSlotsUI()
    {
        foreach (Transform child in containerTransform)
            Destroy(child.gameObject);

        var slots = SQLiteDB.instance.GetAllSaveSlots();

        foreach (var slot in slots)
        {
            GameObject button = Instantiate(slotButtonPrefab, containerTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text =
                $"Partida {slot.id} - {slot.location} - Tiempo: {slot.playTime} - {slot.createdAt}";

            int saveId = slot.id;
            button.GetComponent<Button>().onClick.AddListener(() => OverwriteSave(saveId));
        }
        
        if (slots.Count < 20)
        {
            GameObject newButton = Instantiate(addNewSaveButtonPrefab, containerTransform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = "+ Nueva Partida";
            newButton.GetComponent<Button>().onClick.AddListener(() => CreateNewSave());
        }
    }

    void OverwriteSave(int saveId)
    {
       GameController.instance.SaveCurrentGame(saveId);
        Debug.Log($"Partida sobrescrita en ID: {saveId}");
    }

    void CreateNewSave()
    {
        int newSaveId = SQLiteDB.instance.CreateNewSaveSlot();
        if (newSaveId != -1)
        {
            SQLiteDB.instance.InsertInitialGameData(newSaveId);
            GameController.instance.SaveCurrentGame(newSaveId);
            Debug.Log($"Nueva partida guardada en ID: {newSaveId}");
            LoadSlotsUI(); 
        }
        else
        {
            Debug.LogWarning("Ya tienes 20 partidas guardadas.");
        }
    }
}
