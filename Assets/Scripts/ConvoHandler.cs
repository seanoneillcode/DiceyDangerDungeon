using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoHandler : MonoBehaviour
{
    public Conversation currentConvo;
    private CameraController cameraController;
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        game = FindObjectOfType<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void StartConvo(Conversation convo)
    {
        this.currentConvo = convo;
        cameraController.ZoomToPosition(currentConvo.transform.GetChild(0).transform);
        Vector3 playerPos = game.player.transform.position;
        Vector3 targetPos = currentConvo.transform.position;
        Vector3 direction = (playerPos - targetPos);
        direction.y = 0;
        direction.Normalize();
        Vector3 distanceTarget = targetPos + (direction * 2);
        distanceTarget.y = 0;
        game.MovePlayer(distanceTarget);
    }

    internal void Done()
    {
        currentConvo = null;
        cameraController.ResetPosition();
    }
}
