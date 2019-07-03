using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoHandler : MonoBehaviour
{
    private ReferenceHolder referenceHolder;
    public GameObject infoBox;
    public GameObject bookSource;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = FindObjectOfType<ReferenceHolder>();
        infoBox.SetActive(false);
        bookSource.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.convoHandler.currentGameInfo == null)
        {
            infoBox.SetActive(false);
            bookSource.SetActive(false);
        } else
        {
            infoBox.SetActive(true);
            bookSource.SetActive(true);
            text.text = referenceHolder.convoHandler.currentGameInfo.GetText(); ;
        }
    }

    public void CloseInfo()
    {
        if (referenceHolder.convoHandler.currentGameInfo != null)
        {
            referenceHolder.convoHandler.CloseInfo();
        }
    }
}
