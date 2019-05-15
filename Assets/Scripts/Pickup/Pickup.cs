﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject consumedItem;
    public bool consumed;
    public PickupType type;
    public bool isCurse;
    public GameObject trapActor;

    // Start is called before the first frame update
    void Start()
    {
        consumed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (consumed && consumedItem.activeSelf)
        {
            consumedItem.SetActive(false);
        }
    }

    public void Consume()
    {
        consumed = true;
    }

    public void ApplyPickupToPlayer(Game game)
    {
        switch (this.type)
        {
            case Pickup.PickupType.MAX_HEALTH_INC:
                game.maxPlayerHealth += 1;
                game.playerHealth += 1;
                break;
            case Pickup.PickupType.MAX_HEALTH_DEC:
                game.maxPlayerHealth -= 1;
                break;
            case Pickup.PickupType.FRIEND:
                game.friendHelp += 1;
                break;
            case Pickup.PickupType.GHOST:
                game.ghostHindrence += 1;
                break;
            case Pickup.PickupType.SWORD:
                game.swordHelp += 1;
                break;
            case Pickup.PickupType.ARMOUR:
                game.armourHelp += 1;
                break;
            case Pickup.PickupType.TELEPORT:
                game.TeleportPlayerRandomly();
                break;
            case Pickup.PickupType.TRAP:
                // find actor and enable
                trapActor.SetActive(true);
                GetComponent<Node>().SetRisk(UnityEngine.Random.Range(1, 7));
                break;
        }
    }

    public enum PickupType
    {
        HEALTH,
        MAX_HEALTH_INC,
        MAX_HEALTH_DEC,
        TELEPORT,
        FRIEND,
        ARMOUR,
        SWORD,
        GOLD,
        PRISONER,
        TRAP,
        GHOST
    }
}
