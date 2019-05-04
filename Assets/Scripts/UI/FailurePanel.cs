using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailurePanel : MonoBehaviour
{
    private ReferenceHolder referenceHolder;
    private GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        panel = transform.GetChild(0).gameObject;
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.game.isPlayerDead())
        {
            panel.SetActive(true);
        }
    }
}
