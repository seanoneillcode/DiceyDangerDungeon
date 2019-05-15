using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public Node nodeA;
    public Node nodeB;

    private void Start()
    {
        //SetNodes();
    }

    private void SetNodes()
    {
        BoxCollider thisCollider = gameObject.GetComponent<BoxCollider>();


        Collider[] colliders = Physics.OverlapBox(
            transform.localPosition + thisCollider.center,
            thisCollider.bounds.extents * 2,
            Quaternion.identity,
            LayerMask.GetMask("Nodes"));
        
        foreach (Collider collider in colliders)
        {
            Node node = collider.gameObject.GetComponent<Node>();
            if (node != null)
            {
                if (nodeA == null)
                {
                    nodeA = node;
                }
                else {
                    nodeB = node;
                }
                node.AddLink(this);
            }
        }
    }

    public Node GetOtherNode(Node otherNode)
    {
        return otherNode.Equals(nodeA) ? nodeB : nodeA;
    }
}
