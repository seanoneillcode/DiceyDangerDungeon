using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public int risk;
    public Actor actor;

    // Start is called before the first frame update
    void Start()
    {
        Text riskText = GetComponentInChildren<Text>();
        if (riskText != null) {
            if (risk > 0)
            {
                riskText.text = "" + risk;
            } else
            {
                riskText.gameObject.SetActive(false);
            }
        }
        actor = GetOverlap(this);
        if (actor != null)
        {
            actor.node = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RemoveRisk()
    {
        risk = 0;
        Text riskText = GetComponentInChildren<Text>();
        if (riskText != null)
        {
            riskText.gameObject.SetActive(false);
        }
    }

    private Actor GetOverlap(Node node)
    {
        GameObject actorCollider = node.gameObject;
        Collider[] colliders = Physics.OverlapBox(actorCollider.transform.position, actorCollider.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Actors"));
        foreach (Collider collider in colliders)
        {
            Actor actor = collider.gameObject.GetComponent<Actor>();
            if (actor != null)
            {
                if (collider.gameObject.GetComponent<Player>() == null)
                {
                    Debug.Log("actor WAS found for " + gameObject.name);
                    return actor;
                }
            }
        }
        Debug.Log("no actor found for " + gameObject.name);
        return null;
    }
}
