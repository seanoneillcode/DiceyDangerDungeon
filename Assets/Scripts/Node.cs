using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public int risk;
    public Actor actor;
    public List<Link> links;
    public Pickup pickup;
    public Character character;

    // Start is called before the first frame update
    void Start()
    {
        if (links == null)
        {
            links = new List<Link>();
        }
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
        pickup = GetComponent<Pickup>();
        character = GetComponentInChildren<Character>();
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

    internal void AddLink(Link link)
    {
        if (this.links == null)
        {
            this.links = new List<Link>();
        }
        this.links.Add(link);
    }

    public bool hasAction()
    {
        bool hasPickup = pickup != null && !pickup.consumed;
        bool hasCharacter = character != null;
        return actor != null || hasPickup || hasCharacter;
    }

    internal void HandleRoll(bool didSucceed, Game game)
    {
        if (actor != null)
        {
            if (didSucceed)
            {
                game.ExplodePosition(actor.gameObject.transform.position);
                Destroy(actor.gameObject);
                actor = null;
                RemoveRisk();
            } else
            {
                game.playerHealth -= 1;
            }
        }
        if (pickup != null && !pickup.consumed)
        {
            pickup.Consume();
            game.ExplodePosition(pickup.gameObject.transform.position + new Vector3(0, 0.4f, 0.4f));
            RemoveRisk();
            if ((!pickup.isCurse && didSucceed) || (pickup.isCurse && !didSucceed))
            {
                pickup.ApplyPickupToPlayer(game);
            }
            if (character != null)
            {
                if ((didSucceed && !pickup.isCurse) || (!didSucceed && pickup.isCurse))
                {
                    character.SetFollow(game.player.transform);
                }
                else
                {
                    Destroy(character.gameObject);
                    character = null;
                }
            }
        }
    }
}
