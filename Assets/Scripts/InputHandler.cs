using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Nodes", "Actors")))
            {
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
                game.SelectNode(node);

                if (node == null && player != null)
                {
                    
                    game.SelectPlayer(player);
                }
            } else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                //game.SelectPlayer(null);
                game.SelectNode(null);
            }
        }
    }
}
