using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentLevelPanel : MonoBehaviour
{
    private Text text;
    private ReferenceHolder referenceHolder;

    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float percentageComplete = Mathf.Ceil((StaticState.currentLevel) / ((float) referenceHolder.game.bossLevel) * 100f);
        if (percentageComplete == 100f)
        {
            text.text = "BOSS";
        } else
        {
            if (percentageComplete == 0f)
            {
                text.text = "HOME";
            } else
            {
                text.text = "" + percentageComplete + "%";
            }
        }

    }
}
