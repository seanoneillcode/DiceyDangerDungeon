using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector]  public Actor selectedActor;
    [HideInInspector]  public Node selectedNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SelectActor(Actor actor)
    {
        selectedActor = actor;
    }

    internal void SelectNode(Node node)
    {
        if (selectedActor != null)
        {
            selectedActor.targetPos = node.transform.position;
            selectedActor = null;
        }
        selectedNode = node;
    }
}
