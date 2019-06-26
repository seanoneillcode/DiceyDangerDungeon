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

    private Color32 myGreen = new Color32(99, 199, 77, 255);
    private Color32 myRed = new Color32(158, 40, 53, 255);


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
                rollText.text = "" + (referenceHolder.game.GetDiceRoll());
            }
            else
            {
                rollText.text = "";
            }
            if (referenceHolder.game.DidMissEnemy())
            {
                missedImage.SetActive(true);
                missedImage.GetComponent<Animator>().Play("missed-ui");
                rollText.color = myRed;
            } else
            {
                missedImage.SetActive(false);
                if (referenceHolder.game.DidWinRoll()) {
                    rollText.color = myGreen;
                }
                else
                {
                    rollText.color = Color.white;
                }
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
