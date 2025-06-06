using UnityEngine;

[System.Serializable]
public class SubQuest
{
    public string id;
    public string description;
    public bool completed;
    public bool isActive = false;
    public string nextSubQuestId; 
}