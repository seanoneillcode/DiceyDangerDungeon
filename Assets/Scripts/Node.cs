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
}
