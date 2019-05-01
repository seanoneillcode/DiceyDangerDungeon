using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector]  public Actor selectedActor;
    [HideInInspector]  public Node selectedNode;

    public int playerHealth = 5;
    public int diceResult = -1;

    private Actor activeActor;
    private ExplosionHandler explosionHandler;

    // Start is called before the first frame update
    void Start()
    {
        explosionHandler = FindObjectOfType<ExplosionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeActor != null)
        {
            if (activeActor.IsAtTarget())
            {
                Debug.Log("perform the action at the node");
                Node node = GetOverlap(activeActor);
                if (node != null && node.risk > 0)
                {
                    Debug.Log("node is " + node.gameObject.name);
                    diceResult = UnityEngine.Random.Range(1, 6);
                    Debug.Log("rolled a " + diceResult);
                    Debug.Log("risk was " + node.risk)
                    if (diceResult < node.risk)
                    {
                        Debug.Log("hurt the player ");
                        playerHealth = playerHealth - 1;
                    }
                    node.RemoveRisk();
                    if (node.actor != null)
                    {
                        explosionHandler.Explode(node.actor.gameObject.transform.position);
                        Destroy(node.actor.gameObject);
                    } else
                    {
                        Debug.Log("Node doesn't have an actor");
                    }
                }
                activeActor = null;
            }
        }
    }

    private Node GetOverlap(Actor actor)
    {
        //BoxCollider actorCollider = actor.GetComponent<BoxCollider>();
        GameObject actorCollider = actor.gameObject;
        Collider[] colliders = Physics.OverlapBox(actorCollider.transform.position, actorCollider.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Nodes"));
        foreach (Collider collider in colliders)
        {
            Node node = collider.gameObject.GetComponent<Node>();
            if (node != null)
            {
                return node;
            }
        }
        return null;
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
