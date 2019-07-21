using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechHandler : MonoBehaviour
{

    Conversation currentConvo;

    public GameObject speechBubble;
    public GameObject avatar;

    private int lineIndex;
    public Text speechText;

    private ReferenceHolder referenceHolder;

    // Use this for initialization
    void Start()
    {
        referenceHolder = FindObjectOfType<ReferenceHolder>();
        lineIndex = 0;
        //currentConvo = referenceHolder.convoHandler != null ? referenceHolder.convoHandler.currentConvo : null;
        UpdateSpeechBubble();
    }

    void Update()
    {
        if (referenceHolder.convoHandler.currentConvo == null)
        {
            speechBubble.SetActive(false);
            avatar.SetActive(false);
        } else
        {
            currentConvo = referenceHolder.convoHandler.currentConvo;
            speechBubble.SetActive(true);
            avatar.SetActive(true);
            UpdateSpeechBubble();
        }
    }

    public void CancelDialog()
    {
        speechBubble.SetActive(false);
        avatar.SetActive(false);
        referenceHolder.convoHandler.Done();
        lineIndex = 0;
        currentConvo = null;
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
            avatar.SetActive(false);
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

    }
}
