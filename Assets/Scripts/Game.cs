using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [HideInInspector]  public Player selectedPlayer;
    [HideInInspector]  public Node selectedNode;

    public int playerHealth = 2;
    private int diceResult = -100;
    public int finalDiceRoll = -100;
    public Goal reachedGoal;
    public GameObject map;

    public int friendHelp = 0;
    public int ghostHindrence = 0;
    public int armourHelp = 0;
    public int swordHelp = 0;

    public Player player;
    private ExplosionHandler explosionHandler;
    public Node actionedNode;
    public bool hasRolled;
    public bool isRolling;


    public bool hitEnemy;
    public int maxPlayerHealth = 5;
    internal bool canRun;
    private Vector3 lastValidPosition;


    internal int bossLevel = 10;
    public bool didSucceed;


    private List<Node> waypoints;
    public bool celebrating;

    // Start is called before the first frame update
    void Start()
    {
        explosionHandler = FindObjectOfType<ExplosionHandler>();
        reachedGoal = null;
        hasRolled = false;
        hitEnemy = false;
        canRun = false;
        maxPlayerHealth = StaticState.permHealthBonus + maxPlayerHealth;
        playerHealth = StaticState.permHealthBonus + playerHealth;
        armourHelp = StaticState.permShieldBonus + armourHelp;
        friendHelp = StaticState.permRollBonus + friendHelp;
        if (player != null)
        {
            selectedPlayer = player;
            lastValidPosition = player.transform.position;
        }
        waypoints = new List<Node>();
    }

    internal void EmbarkOnNextLevel()
    {
        StaticState.currentLevel += 1;
        if (StaticState.currentLevel > bossLevel)
        {
            StaticState.currentLevel = 0;
        }
    }

    internal void MovePlayer(Vector3 position)
    {
        Debug.Log("moving player");
        if (selectedPlayer != null)
        {
            selectedPlayer.targetPos = position;
        }
    }

    internal string GetNextLevelName()
    {
        if (StaticState.currentLevel == bossLevel)
        {
            return "BossScene";
        }
        if (StaticState.currentLevel == -1)
        {
            return "HomeScene";
        }
        return "GeneratedLevel";
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
                        reachedGoal = goal;
                    }
                    Pickup pickup = node.pickup;
                    if (pickup != null && !pickup.consumed)
                    {
                        if (pickup.type == Pickup.PickupType.HEALTH)
                        {
                            pickup.Consume();
                            node.UpdateRiskText();
                            explosionHandler.PickupPlant(pickup.gameObject.transform.position + new Vector3(0, 0.4f, 0.4f));
                            playerHealth += pickup.amount;
                        }
                    }
                    GameInfo gameInfo = node.gameObject.GetComponentInChildren<GameInfo>();
                    if (gameInfo != null && !gameInfo.isShown)
                    {
                        ConvoHandler convoHandler = FindObjectOfType<ConvoHandler>();
                        convoHandler.ShowInfo(gameInfo);
                        gameInfo.Consume();
                    }
                }

            }
            if (waypoints.Count > 0)
            {
                if (waypoints.Count > 1)
                {
                    player.targetPos = waypoints[waypoints.Count - 1].transform.position;
                } else
                {
                    if (selectedNode != null && selectedNode.risk > 0)
                    {
                        Vector3 direction = Vector3.Normalize(selectedNode.transform.position - selectedPlayer.transform.position);
                        selectedPlayer.targetPos = selectedNode.transform.position - (direction * 1f);
                    }
                    else
                    {
                        player.targetPos = waypoints[waypoints.Count - 1].transform.position;
                    }
                }
                if (player.IsAtTarget())
                {
                    waypoints.Remove(waypoints[waypoints.Count - 1]);
                }
            }
        }
        if (isRolling)
        {
            diceResult = UnityEngine.Random.Range(1, 7);
        }
        else
        {
            diceResult = -100;
        }
    }

    internal void PlayerCelebrate()
    {
        celebrating = true;
        StartCoroutine(ExecuteAfterTime(2f, () => {
            celebrating = false;
        }));
    }

    internal void CancelEmbark()
    {
        RunAway();
        reachedGoal = null;
    }

    internal void RunAway()
    {
        if (lastValidPosition != null)
        {
            selectedNode = null;
            finalDiceRoll = -100;
            actionedNode = null;
            hasRolled = false;
            player.targetPos = lastValidPosition;
            canRun = false;
        }
    }

    internal bool HasDiceRoll()
    {
        return GetDiceRoll() > -50;
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
            this.didSucceed = GetDiceRollWithmodifiers() >= actionedNode.risk;
        }));

        StartCoroutine(ExecuteAfterTime(0.8f, () => {
            ResolveRolls(actionedNode);
        }));

        StartCoroutine(ExecuteAfterTime(1.6f, () => {
            if (actionedNode != null && !actionedNode.hasAction())
            {
                if (playerHealth > 0)
                {
                    MovePlayerAction(player, actionedNode);
                }
            }
            if (actionedNode != null)
            {
                if (actionedNode.risk < 1)
                {
                    actionedNode = null;
                }
            }
            finalDiceRoll = -100;
            hasRolled = false;
        }));
    }

    private List<Node> GetNodeWaypoints(Node currentNode, Node target, List<Node> nodesSoFar)
    {
        nodesSoFar.Add(currentNode);
        List<Node> connectedNodes = new List<Node>();

        foreach (Link link in currentNode.links)
        {
            if (link.nodeA == target || link.nodeB == target)
            {
                connectedNodes.Add(target);
                connectedNodes.Add(currentNode);
                return connectedNodes;
            }
        }
        
        foreach (Link link in currentNode.links)
        {
            if (link.nodeA != currentNode && !nodesSoFar.Contains(link.nodeA) && link.nodeA.risk < 1)
            {
                List<Node> waypoints = GetNodeWaypoints(link.nodeA, target, nodesSoFar);
                if (waypoints.Count > 0)
                {
                    waypoints.Add(currentNode);
                    return waypoints;
                }
            }
            if (link.nodeB != currentNode && !nodesSoFar.Contains(link.nodeB) && link.nodeB.risk < 1)
            {
                List<Node> waypoints = GetNodeWaypoints(link.nodeB, target, nodesSoFar);
                if (waypoints.Count > 0)
                {
                    waypoints.Add(currentNode);
                    return waypoints;
                }
            }
        }

        return connectedNodes;
    }

    private void ResolveRolls(Node node)
    {
        this.didSucceed = false;
        node.HandleRoll(GetDiceRollWithmodifiers() >= node.risk, this);
        if (playerHealth < 1)
        {
            explosionHandler.Explode(player.gameObject.transform.position);
            Destroy(player.gameObject);
            player = null;
            hitEnemy = true;
            actionedNode = null;
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

    public List<Node> GetNodePlayerCanReach(Node node)
    {
        Node playerNode = GetOverlap(selectedPlayer);
        if (playerNode == null)
        {
            return new List<Node>();
        }
        List<Node> nodes = GetNodeWaypoints(playerNode, node, new List<Node>());
        nodes.Remove(playerNode);
        return nodes;
    }

    internal void SelectPlayer(Player player)
    {
        selectedPlayer = player;
    }

    internal void MoveToNode(Node node)
    {
        List<Node> reachableNodes = GetNodePlayerCanReach(node);
        if (reachableNodes.Count < 1)
        {
            return;
        }

        if (actionedNode != null && actionedNode.risk > 0) {
            return;
        }


        if (selectedPlayer != null && node != null && actionedNode == null)
        {
            lastValidPosition = selectedPlayer.transform.position;
        }
        selectedNode = node;
        waypoints = reachableNodes;
    }

    internal void TeleportPlayerRandomly()
    {
        explosionHandler.PortalExplosion(player.transform.position);
        explosionHandler.PortalExplosion(player.transform.position + new Vector3(0.8f, 0, 0));
        explosionHandler.PortalExplosion(player.transform.position + new Vector3(0, 0, -0.8f));
        actionedNode = null;
        selectedNode = null;
        finalDiceRoll = -100;
        hasRolled = false;
        List<Transform> children = new List<Transform>();
        foreach (Transform child in map.transform)
        {
            if (child.gameObject.GetComponent<Node>() != null)
            {
                children.Add(child);
            }
        }
        int choiceIndex = UnityEngine.Random.Range(0, children.Count);
        player.Teleport(children[choiceIndex].position);
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

    public bool DidWinRoll()
    {
        return GetDiceRollWithmodifiers() > -1 && GetDiceRollWithmodifiers() > actionedNode.risk;
    }
}
