using System.Collections.Generic;
using UnityEngine;
public class NPCDialogueTrigger : MonoBehaviour
{
    public int characterID;

    public void TriggerDialogue()
    {
        var lines = SQLiteDB.instance.GetRandomDialogue(characterID);
        DialogueManager.instance.StartDialogue(new DialogueData(lines));
    }
}

public class DialogueData
{
    public List<string> lines;

    public DialogueData(List<string> lines)
    {
        this.lines = lines;
    }
}