using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector]  public Actor selectedActor;
    [HideInInspector]  public Node selectedNode;

    public int playerHealth = 5;

    private Actor activeActor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (activeActor != null)
        {
            if (activeActor.IsAtTarget())
            {
                Debug.Log("perform the action at the node");
                activeActor = null;
            }
        }
    }

    internal void SelectActor(Actor actor)
    {
        selectedActor = actor;
    }

    internal void SelectNode(Node node)
    {
        if (selectedActor != null && node != null)
        {
            activeActor = selectedActor;
            Vector3 direction = Vector3.Normalize(node.transform.position - selectedActor.transform.position);
            selectedActor.targetPos = node.transform.position - (direction * 0.8f);
            selectedActor = null;
        }
        selectedNode = node;
    }
}
