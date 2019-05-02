using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector]  public Player selectedPlayer;
    [HideInInspector]  public Node selectedNode;

    public int playerHealth = 5;
    public int diceResult = -1;

    private Player activePlayer;
    private ExplosionHandler explosionHandler;
    private Node actionedNode;

    // Start is called before the first frame update
    void Start()
    {
        explosionHandler = FindObjectOfType<ExplosionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activePlayer != null)
        {
            if (activePlayer.IsAtTarget())
            {
                Node node = GetOverlap(activePlayer);
                if (node != null && node.risk > 0 && node != actionedNode)
                {
                    Debug.Log("perform the action at the node");
                    Debug.Log("node is " + node.gameObject.name);

                    actionedNode = node;

                    StartCoroutine(ExecuteAfterTime(0.5f, () => {
                        RollDiceStep(node);
                    }));

                    StartCoroutine(ExecuteAfterTime(1.5f, () => {
                        RemoveActorStep(node);
                    }));

                    StartCoroutine(ExecuteAfterTime(2.5f, () => {
                        MovePlayerAction(activePlayer, node);
                        actionedNode = null;
                        diceResult = -1;
                    }));
                }
            }
        }
    }

    private void RollDiceStep(Node node)
    {
        diceResult = UnityEngine.Random.Range(1, 6);
        Debug.Log("rolled a " + diceResult);
        Debug.Log("risk was " + node.risk);

        if (diceResult < node.risk)
        {
            Debug.Log("hurt the player ");
            playerHealth = playerHealth - 1;
        }
    }

    private void RemoveActorStep(Node node)
    {
        if (node.actor != null)
        {
            explosionHandler.Explode(node.actor.gameObject.transform.position);
            Destroy(node.actor.gameObject);
        }
        else
        {
            Debug.Log("Node doesn't have an actor");
        }
        node.RemoveRisk();
    }

    private void MovePlayerAction(Player player, Node node) {
        player.targetPos = node.transform.position;
    }

    private Node GetOverlap(Player player)
    {
        GameObject actorCollider = player.gameObject;
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

    internal void SelectPlayer(Player player)
    {
        selectedPlayer = player;
    }

    internal void SelectNode(Node node)
    {
        if (selectedPlayer != null && node != null)
        {
            activePlayer = selectedPlayer;
            if (node.actor != null)
            {
                Vector3 direction = Vector3.Normalize(node.transform.position - selectedPlayer.transform.position);
                selectedPlayer.targetPos = node.transform.position - (direction * 0.8f);
            } else
            {
                selectedPlayer.targetPos = node.transform.position;
            }
        }
        selectedNode = node;
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }
}
