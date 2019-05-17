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
    public Text riskText;

    // Start is called before the first frame update
    void Start()
    {
        if (links == null)
        {
            links = new List<Link>();
        }
        riskText = GetComponentInChildren<Text>();
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
        if (riskText != null)
        {
            riskText.gameObject.SetActive(false);
        }
    }

    public void SetRisk(int newRisk)
    {
        risk = newRisk;
        if (riskText != null)
        {
            riskText.gameObject.SetActive(true);
            riskText.text = "" + newRisk;
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
        
        if (pickup != null && !pickup.consumed)
        {
            pickup.Consume();
            game.ExplodePosition(pickup.gameObject.transform.position + new Vector3(0, 0.4f, 0.4f));
            RemoveRisk();
            if (pickup.isCurse)
            {
                if (!didSucceed)
                {
                    pickup.ApplyPickupToPlayer(game);
                    if (character != null)
                    {
                        character.SetFollow(game.player.transform);
                    }
                }
                else
                {
                    if (character != null)
                    {
                        Destroy(character.gameObject);
                        character = null;
                    }
                }
            }
            else {
                if (didSucceed)
                {
                    pickup.ApplyPickupToPlayer(game);
                    if (character != null)
                    {
                        character.SetFollow(game.player.transform);
                    }
                } else
                {
                    if (character != null)
                    {
                        Destroy(character.gameObject);
                        character = null;
                    }
                }
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
        if (actor != null)
        {
            if (didSucceed)
            {
                game.ExplodePosition(actor.gameObject.transform.position);
                Destroy(actor.gameObject);
                actor = null;
                game.canRun = false;
                RemoveRisk();
            }
            else
            {
                if (game.armourHelp > 0)
                {
                    game.armourHelp -= 1;
                }
                else
                {
                    game.canRun = true;
                    game.playerHealth -= 1;
                }
            }
        }
    }
}
