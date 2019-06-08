using System.Collections;
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
    public GameObject runButton;
    public GameObject missedImage;
    public GameObject friend;
    public GameObject ghost;

    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        panel = transform.GetChild(0).gameObject;
        panel.SetActive(false);
        missedImage.SetActive(false);
        runButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.game.actionedNode != null)
        {
            panel.SetActive(true);
            riskText.text = "" + referenceHolder.game.actionedNode.risk;
            if (referenceHolder.game.HasDiceRoll())
            {
                Debug.Log("roll text is:" + referenceHolder.game.GetDiceRoll());
                rollText.text = "" + (referenceHolder.game.GetDiceRoll());
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
            if (referenceHolder.game.friendHelp > 0)
            {
                friend.SetActive(true);
                friend.GetComponentInChildren<Text>().text = "+" + referenceHolder.game.friendHelp;
            }
            else {
                friend.SetActive(false);
            }
            if (referenceHolder.game.ghostHindrence > 0)
            {
                ghost.SetActive(true);
                ghost.GetComponentInChildren<Text>().text = "-" + referenceHolder.game.ghostHindrence;
            }
            else
            {
                ghost.SetActive(false);
            }
            if (referenceHolder.game.canRun)
            {
                runButton.SetActive(true);
            } else
            {
                runButton.SetActive(false);
            }
        }
        else
        {
            panel.SetActive(false);
        }
        
        
    }

    public void RunAway()
    {
        referenceHolder.game.RunAway();
    }

    public void RollDice()
    {
        referenceHolder.game.RollDice();
    }
}
