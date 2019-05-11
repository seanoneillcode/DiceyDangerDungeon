using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector]  public Player selectedPlayer;
    [HideInInspector]  public Node selectedNode;

    public int playerHealth = 2;
    private int diceResult = -1;
    public int finalDiceRoll = -1;
    public bool hasReachedGoal;

    public Player player;
    private ExplosionHandler explosionHandler;
    public Node actionedNode;
    private bool hasRolled;
    private bool isRolling;
    private bool hitEnemy;
    public int maxPlayerHealth = 5;

    // Start is called before the first frame update
    void Start()
    {
        explosionHandler = FindObjectOfType<ExplosionHandler>();
        hasReachedGoal = false;
        hasRolled = false;
        hitEnemy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.IsAtTarget())
            {
                Node node = GetOverlap(player);
                if (node != null && node.risk > 0 && node != actionedNode)
                {
                    Debug.Log("perform the action at the node");
                    Debug.Log("node is " + node.gameObject.name);

                    actionedNode = node;
                }
                if (node != null)
                {
                    Goal goal = node.gameObject.GetComponent<Goal>();
                    if (goal != null)
                    {
                        hasReachedGoal = true;
                    }
                    Pickup pickup = node.gameObject.GetComponent<Pickup>();
                    if (pickup != null && !pickup.consumed)
                    {
                        if (pickup.type == Pickup.PickupType.HEALTH)
                        {
                            pickup.Consume();
                            explosionHandler.PickupPlant(pickup.gameObject.transform.position + new Vector3(0, 0.4f, 0.4f));
                            playerHealth++;
                        }
                    }
                }

            }
        }
        if (isRolling)
        {
            diceResult = UnityEngine.Random.Range(1, 7);
        }
        else
        {
            diceResult = -1;
        }
        if (playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }
    }

    public int GetDiceRoll()
    {
        if (finalDiceRoll > 0)
        {
            return finalDiceRoll;
        } else
        {
            return diceResult;
        }
    }

    internal void RollDice()
    {
        if (actionedNode == null || hasRolled)
        {
            return;
        }
        hasRolled = true;
        isRolling = true;
        StartCoroutine(ExecuteAfterTime(0.4f, () =>
        {
            isRolling = false;
            hitEnemy = false;
            finalDiceRoll = UnityEngine.Random.Range(1, 7);
            if (playerHealth == 1 && finalDiceRoll < actionedNode.risk)
            {
                finalDiceRoll = UnityEngine.Random.Range(1, 7);
            }
        }));

        StartCoroutine(ExecuteAfterTime(0.8f, () => {
            ResolveRolls(actionedNode);
        }));

        StartCoroutine(ExecuteAfterTime(1.6f, () => {
            if (hitEnemy)
            {
                if (playerHealth > 0)
                {
                    MovePlayerAction(player, actionedNode);
                }
            }
            finalDiceRoll = -1;
            actionedNode = null;
            hasRolled = false;
        }));
    }

    public bool IsConnected(Node nodeA, Node nodeB)
    {
        foreach( Link link in nodeA.links)
        {
            if (link.nodeA == nodeB || link.nodeB == nodeB)
            {
                return true;
            }
        }
        return false;
    }

    private void ApplyPickupToPlayer(Pickup pickup)
    {
        switch (pickup.type)
        {
            case Pickup.PickupType.MAX_HEALTH_INC:
                maxPlayerHealth += 1;
                playerHealth += 1;
                break;
            case Pickup.PickupType.MAX_HEALTH_DEC:
                maxPlayerHealth -= 1;
                break;
        }
    }

    private void ResolveRolls(Node node)
    {
        if (finalDiceRoll < node.risk)
        {
            if (node.actor != null)
            {
                playerHealth = playerHealth - 1;
            }
            Pickup pickup = node.gameObject.GetComponent<Pickup>();
            if (pickup != null && !pickup.consumed)
            {
                pickup.Consume();
                explosionHandler.PickupPlant(pickup.gameObject.transform.position + new Vector3(0, 0.4f, 0.4f));
                hitEnemy = true;
                node.RemoveRisk();
                if (pickup.isCurse)
                {
                    ApplyPickupToPlayer(pickup);
                }
            }
        }
        else {
            hitEnemy = true;
            if (node.actor != null)
            {
                explosionHandler.Explode(node.actor.gameObject.transform.position);
                Destroy(node.actor.gameObject);
            }
            Pickup pickup = node.gameObject.GetComponent<Pickup>();
            if (pickup != null && !pickup.consumed)
            {
                pickup.Consume();
                explosionHandler.PickupPlant(pickup.gameObject.transform.position + new Vector3(0, 0.4f, 0.4f));
                if (!pickup.isCurse)
                {
                    ApplyPickupToPlayer(pickup);
                }
            }
            node.RemoveRisk();
        }
        if (playerHealth < 1)
        {
            explosionHandler.Explode(player.gameObject.transform.position);
            Destroy(player.gameObject);
            player = null;
            hitEnemy = true;
        }
    }

    private void MovePlayerAction(Player player, Node node) {
        player.targetPos = node.transform.position;
    }

    private Node GetOverlap(Player player)
    {
        if (player == null)
        {
            return null;
        }
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

    public bool CanHighlightNode(Node node)
    {
        Node playerNode = GetOverlap(selectedPlayer);
        return playerNode != null && IsConnected(playerNode, node);
    }

    internal void SelectPlayer(Player player)
    {
        selectedPlayer = player;
    }

    internal void SelectNode(Node node)
    {
        if (!CanHighlightNode(node))
        {
            return;
        }

        if (selectedPlayer != null && node != null && actionedNode == null)
        {
            if (node.actor != null && selectedPlayer.IsAtTarget())
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

    public bool isPlayerDead()
    {
        return playerHealth < 1 && actionedNode == null;
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }

    public bool DidMissEnemy()
    {
        return !hitEnemy && finalDiceRoll > -1 && finalDiceRoll < actionedNode.risk;
    }
}
