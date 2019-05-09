﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollHandler : MonoBehaviour
{
    private ReferenceHolder referenceHolder;
    private GameObject panel;
    public Text riskText;
    public Text rollText;
    public GameObject rollButton;
    public GameObject missedImage;

    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        panel = transform.GetChild(0).gameObject;
        panel.SetActive(false);
        missedImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.game.actionedNode != null)
        {
            panel.SetActive(true);
            riskText.text = "" + referenceHolder.game.actionedNode.risk;
            if (referenceHolder.game.GetDiceRoll() > 0)
            {
                rollText.text = "" + referenceHolder.game.GetDiceRoll();
            }
            else
            {
                rollText.text = "";
            }
            if (referenceHolder.game.DidMissEnemy())
            {
                missedImage.SetActive(true);
            } else
            {
                missedImage.SetActive(false);
            }
        }
        else
        {
            panel.SetActive(false);
        }
        
        
    }

    public void RollDice()
    {
        referenceHolder.game.RollDice();
    }
}
