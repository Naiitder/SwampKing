using System;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] private string idQuest;
    [SerializeField] private string subIdQuest;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestManager.instance.ActivateSubQuest(idQuest, subIdQuest);
            Destroy(gameObject);
        }
    }
}
