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

    public Transform testPos;
    private bool next = false;
    
    Vector3 zoomVector = new Vector3(3, -4.5f, 3);
    internal Vector3 goalPoint;

    private void Start()
    {
        game = FindObjectOfType<Game>();
        newPosition = transform.position;
        oldPosition = transform.position;
        oldCameraPosition = Camera.main.transform.localPosition;
        newCameraPosition = Camera.main.transform.localPosition;
        lockToPosition = false;
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
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2);
        }

        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");
            //Camera.main.transform.position += (Camera.main.transform.forward * 0.5f);
            if (next)
            {
                Debug.Log("reset going to " + oldPosition.x + ":" + oldPosition.y);
                ResetPosition();
            } else
            {
                ZoomToPosition(goalPoint);
            }
            next = !next;
        }

        if (lockToPosition && newPosition != null && !transform.position.Equals(newPosition))
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 2);
        }
    }

    internal void ShowGoal(Vector3 playerPos, Vector3 goalPoint)
    {
        transform.position = playerPos;
        StartCoroutine(ExecuteAfterTime(1f, () => {
            ZoomToPosition(goalPoint);
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
        newPosition = pos + zoomVector;
    }

    public void ResetPosition() {
        newPosition = oldPosition;
        lockToPosition = false;
    }

}
