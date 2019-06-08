using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Node leftMinionNode;
    public Node rightMinionNode;

    private bool destroyedLeft;
    private bool destroyedRight;
    private Node node;
    private GameObject self;

    private GameObject leftMinion;
    private GameObject rightMinion;

    // Start is called before the first frame update
    void Start()
    {
        leftMinion = leftMinionNode.GetComponentInChildren<Actor>().gameObject;
        rightMinion = rightMinionNode.GetComponentInChildren<Actor>().gameObject;
        node = GetComponent<Node>();
        destroyedLeft = false;
        destroyedRight = false;
        self = GetComponentInChildren<Actor>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftMinion == null && !destroyedLeft)
        {
            destroyedLeft = true;
            node.SetRisk(node.risk - 1);
            // do shouting boss hurt here
        }
        if (rightMinion == null && !destroyedRight)
        {
            destroyedRight = true;
            node.SetRisk(node.risk - 1);
            // do shouting boss hurt here
        }
        if (self == null)
        {
            destroyedRight = true;
            destroyedLeft = true;
            Destroy(leftMinion);
            Destroy(rightMinion);
            leftMinionNode.RemoveRisk();
            rightMinionNode.RemoveRisk();
        }
    }
}
