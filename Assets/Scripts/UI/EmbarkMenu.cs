using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmbarkMenu : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        if (referenceHolder.game.reachedGoal != null && !once && !referenceHolder.game.isPlayerDead())
        {
            once = true;
            StartCoroutine(ExecuteAfterTime(0.8f));
        }
    }

    public void Toggle()
    {
        this.isVisible = !this.isVisible;
        UpdateVisible();
    }

    public void CancelEmbark()
    {
        referenceHolder.game.CancelEmbark();
        this.isVisible = false;
        once = false;
        UpdateVisible();
    }

    public void Embark()
    {
        referenceHolder.game.EmbarkOnNextLevel();
        SceneManager.LoadScene(referenceHolder.game.GetNextLevelName(), LoadSceneMode.Single);
    }

    public void GoHome()
    {
        StaticState.currentLevel = -1;
        StaticState.Reset();
        SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
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
