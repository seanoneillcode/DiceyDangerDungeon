using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanel : MonoBehaviour
{
    private ReferenceHolder referenceHolder;
    private bool once;
    // Start is called before the first frame update
    void Start()
    {
        referenceHolder = GetComponentInParent<ReferenceHolder>();
        foreach( Transform transform in transform)
        {
            transform.gameObject.SetActive(false);
        }
        once = false;
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

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (Transform transform in transform)
        {
            transform.gameObject.SetActive(true);
        }
    }
}
