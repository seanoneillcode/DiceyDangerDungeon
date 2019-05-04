using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public Node nodeA;
    public Node nodeB;

    bool m_Started;

    private void Start()
    {
        SetNodes();
        m_Started = true;
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

    void OnDrawGizmos()
    {
        BoxCollider thisCollider = gameObject.GetComponent<BoxCollider>();
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.localPosition + thisCollider.center, thisCollider.bounds.extents * 2);
    }

    public Node GetOtherNode(Node otherNode)
    {
        return otherNode.Equals(nodeA) ? nodeB : nodeA;
    }
}
