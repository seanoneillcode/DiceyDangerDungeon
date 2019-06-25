using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceHolder : MonoBehaviour
{
    [HideInInspector]public Game game;
    [HideInInspector] public ConvoHandler convoHandler;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
        convoHandler = FindObjectOfType<ConvoHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
