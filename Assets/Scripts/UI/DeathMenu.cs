using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{

    private ReferenceHolder referenceHolder;
    private bool once;
    private bool isVisible;


    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        isVisible = false;
        once = false;
        UpdateVisible();
    }

    void Update()
    {
        if (referenceHolder.game.isPlayerDead() && !once)
        {
            once = true;
            StartCoroutine(ExecuteAfterTime(0.8f));
        }
    }

    public void Restart()
    {
        StaticState.currentLevel = -1;
        StaticState.Reset();
        SceneManager.LoadScene("BedroomScene", LoadSceneMode.Single);
    }

    public void Toggle()
    {
        this.isVisible = !this.isVisible;
        UpdateVisible();
    }

    public void UpdateVisible()
    {
        foreach (Transform transform in transform)
        {
            transform.gameObject.SetActive(this.isVisible);
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Toggle();
    }
}
