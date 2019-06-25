using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechHandler : MonoBehaviour
{

    Conversation currentConvo;

    public GameObject speechBubble;

    private int lineIndex;
    public Text speechText;

    private float minWidth = 131;
    private float minHeight = 75;

    private float maxLineWidth = 515;
    private float widthStep = 8;
    private float lineHeight = 40;

    private float charsPerLine = 21;

    private ReferenceHolder referenceHolder;

    // Use this for initialization
    void Start()
    {
        referenceHolder = FindObjectOfType<ReferenceHolder>();
        lineIndex = 0;
        currentConvo = referenceHolder.convoHandler.currentConvo;
        UpdateSpeechBubble();
    }

    void Update()
    {
        if (referenceHolder.convoHandler.currentConvo == null)
        {
            speechBubble.SetActive(false);
        } else
        {
            currentConvo = referenceHolder.convoHandler.currentConvo;
            speechBubble.SetActive(true);
            UpdateSpeechBubble();
        }
    }

    public bool IsDone()
    {
        return lineIndex == currentConvo.lines.Count;
    }

    public void NextLine()
    {
        if (currentConvo == null)
        {
            return;
        }
        lineIndex++;
        if (IsDone())
        {
            speechBubble.SetActive(false);
            referenceHolder.convoHandler.Done();
            lineIndex = 0;
            currentConvo = null;
        }
        else
        {
            speechText.text = currentConvo.lines[lineIndex];
            UpdateSpeechBubble();
        }
    }

    private void UpdateSpeechBubble()
    {
        speechText.text = currentConvo != null ? currentConvo.lines[lineIndex] : "";
        RectTransform speechTransform = speechBubble.GetComponent<RectTransform>();
        string dialog = currentConvo != null ? currentConvo.lines[lineIndex] : "";
        int numLines = Mathf.CeilToInt(dialog.Length / charsPerLine);
        float width = maxLineWidth;
        if (numLines <= 1)
        {
            numLines = 1;
            width = 100 + (dialog.Length * 22);
        }
        speechTransform.sizeDelta = new Vector2(width, 35 + (numLines * lineHeight));
    }
}
