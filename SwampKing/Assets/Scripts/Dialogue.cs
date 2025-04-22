using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public List<string> lines;
}

public class NPCDialogueTrigger : MonoBehaviour
{
    public Dialogue dialogueData;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
