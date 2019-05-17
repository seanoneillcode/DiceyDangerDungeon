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

    public int friendHelp = 0;
    public int ghostHindrence = 0;
    public int armourHelp = 0;
    public int swordHelp = 0;

    public Player player;
    private ExplosionHandler explosionHandler;
    public Node actionedNode;
    private bool hasRolled;
    private bool isRolling;
    private bool hitEnemy;
    public int maxPlayerHealth = 5;
    internal bool canRun;
    private Vector3 lastValidPosition;


    // Start is called before the first frame update
    void Start()
    {
        explosionHandler = FindObjectOfType<ExplosionHandler>();
        hasReachedGoal = false;
        hasRolled = false;
        hitEnemy = false;
        canRun = false;
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
                    actionedNode = node;
                }
                if (node != null)
                {
                    Goal goal = node.gameObject.GetComponent<Goal>();
                    if (goal != null)
                    {
                        hasReachedGoal = true;
                    }
                    Pickup pickup = node.pickup;
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

    internal void RunAway()
    {
        if (lastValidPosition != null)
        {
            actionedNode = null;
            selectedNode = null;
            finalDiceRoll = -1;
            actionedNode = null;
            hasRolled = false;
            player.targetPos = lastValidPosition;
            canRun = false;
        }
    }

    public void ExplodePosition(Vector3 pos)
    {
        explosionHandler.PickupPlant(pos);
    }

    internal int GetDiceRollWithmodifiers()
    {
        return finalDiceRoll + friendHelp - ghostHindrence;
    }

    public int GetDiceRoll()
    {
        int result = diceResult;
        if (!isRolling)
        {
            result = GetDiceRollWithmodifiers();
        }
        return result;
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
            if (swordHelp > 0)
            {
                finalDiceRoll = actionedNode.risk;
                swordHelp -= 1;
            }
        }));

        StartCoroutine(ExecuteAfterTime(0.8f, () => {
            ResolveRolls(actionedNode);
        }));

        StartCoroutine(ExecuteAfterTime(1.6f, () => {
            if (!actionedNode.hasAction())
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

    private void ResolveRolls(Node node)
    {
        Debug.Log("rolled a " + GetDiceRollWithmodifiers() + " against a risk of " + node.risk);
        node.HandleRoll(GetDiceRollWithmodifiers() >= node.risk, this);
        if (playerHealth < 1)
        {
            explosionHandler.Explode(player.gameObject.transform.position);
            Destroy(player.gameObject);
            player = null;
            hitEnemy = true;
        }
    }

    private void MovePlayerAction(Player player, Node node) {
        lastValidPosition = player.transform.position;
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
            lastValidPosition = selectedPlayer.transform.position;
            if (node.hasAction() && selectedPlayer.IsAtTarget())
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

    internal void TeleportPlayerRandomly()
    {
        explosionHandler.PortalExplosion(player.transform.position);
        explosionHandler.PortalExplosion(player.transform.position + new Vector3(0.8f, 0, 0));
        explosionHandler.PortalExplosion(player.transform.position + new Vector3(0, 0, -0.8f));
        actionedNode = null;
        selectedNode = null;
        finalDiceRoll = -1;
        actionedNode = null;
        hasRolled = false;
        float x = UnityEngine.Random.Range(0, LevelGenerator.SIZE) * 4;
        float z = UnityEngine.Random.Range(0, LevelGenerator.SIZE) * 4;
        player.Teleport(new Vector3(x, 0, z));
        explosionHandler.PortalExplosion(player.transform.position);
        StartCoroutine(ExecuteAfterTime(0.2f, () => {
            explosionHandler.PortalExplosion(player.transform.position + new Vector3(0, 0, -0.8f));
            explosionHandler.PortalExplosion(player.transform.position + new Vector3(0.8f, 0, 0));
        }));
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
        return !hitEnemy && GetDiceRollWithmodifiers() > -1 && GetDiceRollWithmodifiers() < actionedNode.risk;
    }
}
