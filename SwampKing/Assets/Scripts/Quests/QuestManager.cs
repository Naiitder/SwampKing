using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private PlayerManager playerManager;
    
    public TextMeshProUGUI questText;
    
    public List<Quest> activeQuests = new List<Quest>();
    [SerializeField] private TMP_SpriteAsset gamepadSpriteAsset;
    
    private bool jumpedOnce = false;
    private bool attackedOnce = false;

    private Quest tutorialQuest = new Quest
    {
        id = "TUTORIAL_001",
        title = "Completa el tutorial",
        subQuests = new List<SubQuest>
        {
            new SubQuest
            {
                id = "WASD", description = "Presiona <sprite name=\"KeyboardButtons_W\"><sprite name=\"KeyboardButtons_A\"><sprite name=\"KeyboardButtons_S\"><sprite name=\"KeyboardButtons_D\"> para moverte",
                completed = false, isActive =  true
            },
            new SubQuest { id = "JUMP", description = "Presiona <sprite name=\"KeyboardButtons_Space\"> para saltar", completed = false },
            new SubQuest
            {
                id = "DOUBLE_JUMP", description = "Presiona <sprite name=\"KeyboardButtons_Space\"> otra vez para hacer un doble salto",
                completed = false
            },
            new SubQuest { id = "ATTACK_1", description = "Presiona <sprite name=\"mouse-left\"> para atacar", completed = false },
            new SubQuest
            {
                id = "ATTACK_COMBO", description = "Presiona <sprite name=\"mouse-left\"> otra vez para hacer un combo", completed = false
            },
            new SubQuest
            {
                id = "CHARGE_JUMP", description = "Mantén <sprite name=\"KeyboardButtons_Space\"> para hacer un salto cargado", completed = false
            },
            new SubQuest { id = "AIM", description = "Mantén <sprite name=\"mouse-right\"> para apuntar", completed = false},
            new SubQuest { id = "SHOOT", description = "Presiona <sprite name=\"mouse-left\"> para disparar", completed = false },
            new SubQuest { id = "KILL_BOSS", description = "Derrota al asesino de ranas", completed = false },
            new SubQuest { id = "TALK", description = "Presiona <sprite name=\"KeyboardButtons_E\"> para hablar con el campesino", completed = false },
        }

    };

    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else 
            Destroy(gameObject);
        
        playerManager = FindFirstObjectByType<PlayerManager>();
        activeQuests.Add(tutorialQuest);
        
        questText.spriteAsset = gamepadSpriteAsset; 

    }

    public void CompleteSubQuest(string questId, string subQuestId, bool addNextQuest = false)
    {
        if(!IsSubQuestActive(questId, subQuestId)) return;
        
        Quest quest = activeQuests.Find(q => q.id == questId);
        if (quest != null)
        {
            SubQuest sub = quest.subQuests.Find(s => s.id == subQuestId);
            if (sub != null && !sub.completed)
            {
                sub.completed = true;
                
                if(addNextQuest) quest.ActivateNextSubQuest();
            }
        }
    }
    
    public void ActivateSubQuest(string questId, string subQuestId)
    {
        Quest quest = activeQuests.Find(q => q.id == questId);
        quest?.ActivateSubQuest(subQuestId);
    }
    
    
    void Update()
    {
        if (InputController.instance.MoveAmount != 0)
            CompleteSubQuest("TUTORIAL_001", "WASD");

        if (InputController.instance.IsJumpPressed)
        {
            if (!jumpedOnce && IsSubQuestActive("TUTORIAL_001", "JUMP"))
            {
                jumpedOnce = true;
                CompleteSubQuest("TUTORIAL_001", "JUMP");
            } 
        }
        if (IsSubQuestActive("TUTORIAL_001", "DOUBLE_JUMP") && !playerManager.CanDoubleJump)
        {
            CompleteSubQuest("TUTORIAL_001", "DOUBLE_JUMP");
        }
        
        if (InputController.instance.IsAttackPressed)
        {
            if (!attackedOnce)
            {
                attackedOnce = true;
                CompleteSubQuest("TUTORIAL_001", "ATTACK_1", addNextQuest: true);
            }
            else
            {
                CompleteSubQuest("TUTORIAL_001", "ATTACK_COMBO");
            }
        }
        if (playerManager.IsChargingJumping)
        {
            CompleteSubQuest("TUTORIAL_001", "CHARGE_JUMP");
        }

        if (InputController.instance.IsAimingPressed)
            CompleteSubQuest("TUTORIAL_001", "AIM", addNextQuest: true);

        if (InputController.instance.IsAimingPressed && InputController.instance.IsAttackPressed)
            CompleteSubQuest("TUTORIAL_001", "SHOOT");
        
        UpdateQuestText();
    }

    public bool IsSubQuestActive(string questId, string subQuestId)
    {
        Quest quest = activeQuests.Find(q => q.id == questId);
        if (quest == null) return false;

        SubQuest sub = quest.subQuests.Find(s => s.id == subQuestId);
        return sub != null && sub.isActive && !sub.completed;
    }
    
    private void UpdateQuestText()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var quest in activeQuests)
        {
            sb.AppendLine($"<size=40><color=#FFD700>{quest.title}</color></size>");

            bool hasAnySub = false;
            foreach (var sub in quest.subQuests)
            {
                if (sub.isActive && !sub.completed)
                {
                    sb.AppendLine($"<size=30>• {sub.description}</size>");
                    hasAnySub = true;
                }
            }

            if (hasAnySub) sb.AppendLine(); 
        }

        questText.text = sb.ToString().Trim();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)questText.transform);
    }
}