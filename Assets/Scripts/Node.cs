using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public int risk;

    // Start is called before the first frame update
    void Start()
    {
        Text riskText = GetComponentInChildren<Text>();
        if (riskText != null) {
            riskText.text = "" + risk;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
