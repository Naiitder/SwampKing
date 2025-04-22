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
        
        dialogueBox.SetActive(false);
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
    skipToNext = false;

    dialogueText.text = sentence;
    dialogueText.ForceMeshUpdate();

    TMP_TextInfo textInfo = dialogueText.textInfo;

    // Inicializa todo invisible
    for (int i = 0; i < textInfo.characterCount; i++)
    {
        if (!textInfo.characterInfo[i].isVisible) continue;

        int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[i].vertexIndex;
        Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

        for (int j = 0; j < 4; j++)
            vertexColors[vertexIndex + j].a = 0;
    }
    dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

    float delayBetweenWords = 0.03f; 
    int currentChar = 0;

    while (currentChar < textInfo.characterCount)
    {
        if (skipToNext)
        {
            SetAllAlphaTo(255);
            break;
        }
        
        do
        {
            if (!textInfo.characterInfo[currentChar].isVisible)
            {
                currentChar++;
                continue;
            }

            int meshIndex = textInfo.characterInfo[currentChar].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[currentChar].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (float a = 0; a <= 1f; a += Time.deltaTime * 20f) 
            {
                byte alpha = (byte)Mathf.Clamp(a * 255, 0, 255);
                for (int j = 0; j < 4; j++)
                    vertexColors[vertexIndex + j].a = alpha;

                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                if (skipToNext)
                {
                    SetAllAlphaTo(255);
                    isTyping = false;
                    typingCoroutine = null;
                    DisplayNextSentence();
                    yield break;
                }

                yield return null;
            }

            // Asegura visibilidad
            for (int j = 0; j < 4; j++)
                textInfo.meshInfo[meshIndex].colors32[vertexIndex + j].a = 255;

            currentChar++;

        } while (currentChar < textInfo.characterCount &&
                 !char.IsWhiteSpace(textInfo.characterInfo[currentChar].character));

        dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        yield return new WaitForSeconds(delayBetweenWords);
    }

    isTyping = false;
    typingCoroutine = null;

    yield return new WaitForSeconds(0.5f); 
    if (!skipToNext)
        DisplayNextSentence();
}

void SetAllAlphaTo(byte alpha)
{
    TMP_TextInfo textInfo = dialogueText.textInfo;

    for (int i = 0; i < textInfo.characterCount; i++)
    {
        if (!textInfo.characterInfo[i].isVisible) continue;

        int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[i].vertexIndex;
        Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

        for (int j = 0; j < 4; j++)
        {
            vertexColors[vertexIndex + j].a = alpha;
        }
    }

    dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
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
        if (isTyping)
        {
            skipToNext = true;
        }
        else
        {
            DisplayNextSentence();
        }
    }

}
