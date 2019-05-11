using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lifebar : MonoBehaviour
{
    private ReferenceHolder referenceHolder;

    private Image image;
    private Image bg;


    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        bg = transform.GetChild(0).GetComponent<Image>();
        image = transform.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        bg.fillAmount = (referenceHolder.game.maxPlayerHealth / 10f);
        image.fillAmount = (referenceHolder.game.playerHealth / 10f);
    }
}
