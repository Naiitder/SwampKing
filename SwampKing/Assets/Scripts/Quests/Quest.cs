using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public string id;
    public string title;
    public List<SubQuest> subQuests;

    public bool IsCompleted => subQuests.TrueForAll(sq => sq.completed);

    public void ActivateNextSubQuest()
    {
        SubQuest next = subQuests.Find(sq => !sq.completed && !sq.isActive);
        if (next != null) next.isActive = true;
    }
    
    public void ActivateSubQuest(string subQuestId)
    {
        var sub = subQuests.Find(sq => sq.id == subQuestId);
        if (sub != null && !sub.isActive)
        {
            sub.isActive = true;
        }
    }

    public SubQuest GetCurrentSubQuest()
    {
        return subQuests.Find(sq => sq.isActive && !sq.completed);
    }
}