﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvoHandler : MonoBehaviour
{
    public Conversation currentConvo;
    private CameraController cameraController;
    private Game game;

    public GameInfo currentGameInfo;

    private Vector3 zoomBuffer = new Vector3(0, 0, 0);
    Vector3 zoomVector = new Vector3(1.5f, -2.25f, 1.5f);

    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        game = FindObjectOfType<Game>();
    }

    internal void StartConvo(Conversation convo)
    {
        this.currentConvo = convo;
        cameraController.ZoomToPosition(currentConvo.transform.position + zoomVector);
        if (convo.shouldMovePlayer)
        {
            Vector3 playerPos = game.player.transform.position;
            Vector3 targetPos = currentConvo.transform.position;
            Vector3 direction = (playerPos - targetPos);
            direction.y = 0;
            direction.Normalize();
            Vector3 distanceTarget = targetPos + (direction * 2);
            distanceTarget.y = 0;
            game.MovePlayer(distanceTarget);
        }
    }

    internal void ShowInfo(GameInfo gameInfo)
    {
        currentGameInfo = gameInfo;
    }

    internal void Done()
    {
        currentConvo = null;
        cameraController.ResetPosition();
    }

    internal void CloseInfo()
    {
        currentGameInfo = null;
    }
}
