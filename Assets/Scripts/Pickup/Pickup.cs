﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject consumedItem;
    public bool consumed;
    public PickupType type;

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

    public enum PickupType
    {
        HEALTH,
        MAX_HEALTH_INC,
        MAX_HEALTH_DEC,
        TELEPORT,
        FRIEND,
        GHOST
    }
}