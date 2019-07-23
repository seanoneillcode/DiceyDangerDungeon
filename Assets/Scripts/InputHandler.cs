using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    private Game game;
    private CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
        cameraController = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                int id = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id))
                {
                    return;
                }
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                HandleClickOrTap(ray);

            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            HandleClickOrTap(ray);
        }
    }

    private void HandleClickOrTap(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Nodes", "Actors")))
        {
            cameraController.followPlayer = true;
            Debug.Log("hit node actors");
            Player player = hit.transform.gameObject.GetComponent<Player>();
            Node node = hit.transform.gameObject.GetComponent<Node>();
            if (player == null && node == null)
            {
                Actor actor = hit.transform.gameObject.GetComponent<Actor>();
                if (actor != null && actor.node != null)
                {
                    node = actor.node;
                }
            }
            game.MoveToNode(node);

            if (node == null && player != null)
            {

                game.SelectPlayer(player);
            }
        }
        else
        {
            //game.SelectPlayer(null);
            //game.SelectNode(null);
        }

        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Convo")))
        {
            Debug.Log("hit convo");
            Conversation convo = hit.transform.gameObject.GetComponent<Conversation>();
            if (convo != null)
            {
                convo.StartConvo();
            }
        }

        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
        {
            Debug.Log("hit grounf");
            game.MovePlayer(hit.point);
        }
    }
}
