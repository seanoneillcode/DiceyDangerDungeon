using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour
{
    public List<string> lines;
    private ConvoHandler convoHandler;

    // Start is called before the first frame update
    void Start()
    {
        convoHandler = FindObjectOfType<ConvoHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartConvo()
    {
        Debug.Log("setting convo");
        convoHandler.StartConvo(this);
    }
}
