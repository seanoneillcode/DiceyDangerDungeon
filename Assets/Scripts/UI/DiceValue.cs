using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceValue : MonoBehaviour
{
    private ReferenceHolder referenceHolder;
    private Text numberText;
    private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        panel = transform.GetChild(0).gameObject;
        panel.SetActive(true);
        numberText = GetComponentInChildren<Text>();
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.game.GetDiceRoll() > 0)
        {
            panel.SetActive(true);
            numberText.text = "" + referenceHolder.game.GetDiceRoll();
        } else
        {
            panel.SetActive(false);
        }
    }
}
