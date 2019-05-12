using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheildItemUI : MonoBehaviour
{
    private ReferenceHolder referenceHolder;
    public Text armourAmountText;
    public Text swordAmountText;
    public GameObject swordItem;
    public GameObject armourItem;

    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.game.swordHelp > 0)
        {
            swordItem.SetActive(true);
            swordAmountText.text = "" + referenceHolder.game.swordHelp;
        } else
        {
            swordItem.SetActive(false);
        }
        if (referenceHolder.game.armourHelp > 0)
        {
            armourItem.SetActive(true);
            armourAmountText.text = "" + referenceHolder.game.armourHelp;
        }
        else
        {
            armourItem.SetActive(false);
        }
    }
}
