using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") // this string is your newly created tag
        {
            Debug.Log("triggered!");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
