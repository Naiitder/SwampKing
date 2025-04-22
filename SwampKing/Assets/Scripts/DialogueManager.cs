using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;

    private Queue<string> sentences = new Queue<string>();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipToNext = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueData dialogue)
    {
        dialogueBox.SetActive(true);
        sentences.Clear();

        foreach (var line in dialogue.lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }


    public void DisplayNextSentence()
    {
        if (typingCoroutine != null)
        {
            skipToNext = true;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        skipToNext = false;

        foreach (char letter in sentence)
        {
            if (skipToNext)
            {
                dialogueText.text = sentence;
                break;
            }

            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
        typingCoroutine = null;

        yield return new WaitForSeconds(0.5f); 

        if (!skipToNext)
        {
            DisplayNextSentence();
        }
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);
        dialogueText.text = "";
        sentences.Clear();
        typingCoroutine = null;
        isTyping = false;
    }
    
    public void SkipOrNext()
    {
        DisplayNextSentence();
    }
}
