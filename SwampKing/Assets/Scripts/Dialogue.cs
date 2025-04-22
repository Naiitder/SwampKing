using System.Collections.Generic;
using UnityEngine;
public class NPCDialogueTrigger : MonoBehaviour
{
    public int characterID;

    public void TriggerDialogue()
    {
        var lines = SQLiteDB.instance.GetDialogueLines(characterID);
        DialogueManager.instance.StartDialogue(new DialogueData(lines));
    }
}

public class DialogueData
{
    public List<string> lines;

    public DialogueData(List<string> _lines)
    {
        lines = _lines;
    }
}