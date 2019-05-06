using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanel : MonoBehaviour
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
        if (referenceHolder.game.hasReachedGoal && !once)
        {
            once = true;
            StartCoroutine(ExecuteAfterTime(0.8f));
        }
        if (referenceHolder.game.isPlayerDead() && !once)
        {
            once = true;
            StartCoroutine(ExecuteAfterTime(0.8f));
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
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
