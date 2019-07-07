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

    public void Restart()
    {
        StaticState.currentLevel = -1;
        StaticState.Reset();
        SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
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

    public void ShowOptions()
    {

    }

    public void UpdateVisible()
    {
        foreach (Transform transform in transform)
        {
            transform.gameObject.SetActive(this.isVisible);
        }
    }
}
