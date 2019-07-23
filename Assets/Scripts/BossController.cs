using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    //public Node leftMinionNode;
    //public Node rightMinionNode;

    //private bool destroyedLeft;
    //private bool destroyedRight;
    private Node node;
    private Game game;
    private ConvoHandler convoHandler;
    private Conversation conversation;
    private CameraController cameraController;

    //private GameObject leftMinion;
    //private GameObject rightMinion;

    private bool deathHandled;
    public ExplosionHandler explosionHandler;
    public Transform explodePosition;
    private bool startedConvo;


    // Start is called before the first frame update
    void Start()
    {
        startedConvo = false;
        deathHandled = false;
        convoHandler = FindObjectOfType<ConvoHandler>();
        cameraController = FindObjectOfType<CameraController>();
        conversation = GetComponentInChildren<Conversation>();
        //leftMinion = leftMinionNode.GetComponentInChildren<Actor>().gameObject;
        //rightMinion = rightMinionNode.GetComponentInChildren<Actor>().gameObject;
        node = GetComponent<Node>();
        game = FindObjectOfType<Game>();
        //destroyedLeft = false;
        //destroyedRight = false;
        //self = GetComponentInChildren<Actor>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //if (leftMinion == null && !destroyedLeft)
        //{
        //    destroyedLeft = true;
        //    node.SetRisk(node.risk - 1);
        //    // do shouting boss hurt here
        //}
        //if (rightMinion == null && !destroyedRight)
        //{
        //    destroyedRight = true;
        //    node.SetRisk(node.risk - 1);
        //    // do shouting boss hurt here
        //}
        //if (self == null)
        //{
        //    destroyedRight = true;
        //    destroyedLeft = true;
        //    Destroy(leftMinion);
        //    Destroy(rightMinion);
        //    leftMinionNode.RemoveRisk();
        //    rightMinionNode.RemoveRisk();
        //}

        if (!deathHandled && node.risk == 0)
        {
            deathHandled = true;
            StartBossDeathSequence();
        }

        if (startedConvo && convoHandler.currentConvo == null)
        {
            // convo is over
            StaticState.currentLevel = -1;
            StaticState.Reset();
            SceneManager.LoadScene("CelebrateHome", LoadSceneMode.Single);
        }

    }

    private void StartBossDeathSequence()
    {
        Debug.Log("starting death sequence");

        // explosions
        for (int i = 0; i < 32; i++)
        {
            StartCoroutine(ExecuteAfterTime(UnityEngine.Random.Range(0, 2.4f), () => {
                Vector3 randomPos = explodePosition.position + new Vector3(
                    UnityEngine.Random.Range(0, 2f),
                    UnityEngine.Random.Range(0, 6f),
                    UnityEngine.Random.Range(0, 2f)) - new Vector3(0, 3f, 0);
                explosionHandler.PortalExplosion(randomPos);
            }));
        }

        // cheering animation
        game.celebrating = true;

        // dwarf convo
        StartCoroutine(ExecuteAfterTime(2f, () => {
            cameraController.focusTransform = null;
            convoHandler.StartConvo(conversation);
            startedConvo = true;
        }));

        // fade out to pub

    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }
}
