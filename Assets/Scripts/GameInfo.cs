using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    private List<string> lines = new List<string>() {
        "Libraries hold information about the world including boss weaknesses and tips.",
        "Each Level contains a LEGENDARY item. The effects of these items persist across levels.",
        "There are multiple routes to the exit."
    };
    public bool isShown;

    // Start is called before the first frame update
    void Start()
    {
        isShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal string GetText()
    {
        return lines[StaticState.infoCounter];
    }

    internal void Consume()
    {
        isShown = true;
        StaticState.infoCounter++;
        if (StaticState.infoCounter >= lines.Count)
        {
            StaticState.infoCounter = 0;
        }
    }
}
