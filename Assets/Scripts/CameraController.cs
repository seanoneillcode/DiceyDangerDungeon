using System;
using System.Collections;
using System.Collections.Generic;
using Lovely;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int boundary = 48;
    public int speed = 4;
    public bool mouseMoveEnabled = true;
    private Vector3 dragOrigin;
    private float dragSpeed = 0.5f;
    private Game game;
    private Vector3 targetPos = new Vector3();
    public bool followPlayer;
    public bool lockToPosition;

    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 oldCameraPosition;
    private Vector3 newCameraPosition;

    public bool zoomFightEnabled = true;

    public Transform testPos;
    private bool next = false;
    public bool zoomToGoal = true;
    Vector3 zoomVector = new Vector3(3, -4.5f, 3);
    private bool canZoomToFight;

    public Transform focusTransform;

    Vector3 fightZoom = new Vector3(-1, 0.3f, 0f);

    private void Start()
    {
        game = FindObjectOfType<Game>();
        newPosition = transform.position;
        oldPosition = transform.position;
        oldCameraPosition = Camera.main.transform.localPosition;
        newCameraPosition = Camera.main.transform.localPosition;
        lockToPosition = false;
        canZoomToFight = true;
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }

    void Update()
    {
        Vector3 moveAmount = new Vector3();
        if (mouseMoveEnabled)
        {
            if (Input.mousePosition.x > Screen.width - boundary)
            {
                moveAmount.x += speed * Time.deltaTime;
            }
            if (Input.mousePosition.x < boundary)
            {
                moveAmount.x -= speed * Time.deltaTime;
            }
            if (Input.mousePosition.y > Screen.height - boundary)
            {
                moveAmount.z += speed * Time.deltaTime;
            }
            if (Input.mousePosition.y < boundary)
            {
                moveAmount.z -= speed * Time.deltaTime;
            }

        }

        moveAmount += new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed) * Time.deltaTime;
        transform.position += (transform.right * moveAmount.x);
        transform.position += (transform.forward * moveAmount.z);

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            transform.position += (transform.right * pos.x * -dragSpeed);
            transform.position += (transform.forward * pos.y * -dragSpeed);

            //transform.Translate(move, Space.World);
        }

        if (game.selectedPlayer != null && followPlayer && !lockToPosition)
        {
            targetPos = game.selectedPlayer.transform.position;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
        }

        if (lockToPosition && newPosition != null && !transform.position.Equals(newPosition))
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 0.5f);
        }

        if (game.actionedNode != null && game.actionedNode.risk > 0 && game.actionedNode.actor != null)
        {
            if (canZoomToFight && zoomFightEnabled)
            {
                canZoomToFight = false;
                Debug.Log("zooming in");
                ZoomToPosition(game.actionedNode.gameObject.transform.position + fightZoom + zoomVector);

            }
        } else
        {
            if (!canZoomToFight)
            {
                Debug.Log("zooming out");
                canZoomToFight = true;
                ResetPosition();
            }
        }

        if (focusTransform != null)
        {
            transform.LookAt(focusTransform.position);
        }

        //Touch touch0 = Input.GetTouch(0);
        //Touch touch1 = Input.GetTouch(1);
        //if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
        //{
        //    Vector2 prevTouchPosition0 = touch0.position - touch0.deltaPosition;
        //    Vector2 prevTouchPosition1 = touch1.position - touch1.deltaPosition;
        //    float touchDistance = (touch1.position - touch0.position).magnitude;
        //    float prevTouchDistance = (prevTouchPosition1 - prevTouchPosition1).magnitude;
        //    float touchChangeMultiplier = touchDistance / prevTouchDistance;
        //    Transform childCamera = transform.GetChild(0);
        //    childCamera.Translate(touchChangeMultiplier * 0.01f * Vector3.forward, Space.Self);
        //}

    }

    internal void ShowGoal(Vector3 playerPos, Vector3 goalPoint)
    {
        transform.position = playerPos;
        if (!zoomToGoal)
        {
            return;
        }
        StartCoroutine(ExecuteAfterTime(1f, () => {
            ZoomToPosition(goalPoint + zoomVector);
        }));
        StartCoroutine(ExecuteAfterTime(3f, () => {
            ResetPosition();
        }));
    }

    public void ZoomToPosition(Vector3 pos)
    {
        Debug.Log(" going to " + pos.x + ":" + pos.y);
        oldPosition = transform.position;
        lockToPosition = true;
        newPosition = pos;
    }

    public void ResetPosition() {
        newPosition = oldPosition;
        lockToPosition = false;
    }

}
