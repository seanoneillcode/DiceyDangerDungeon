using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Nodes", "Actors")))
            {
                Node node = hit.transform.gameObject.GetComponent<Node>();
                game.SelectNode(node);

                if (node == null)
                {
                    Actor actor = hit.transform.gameObject.GetComponent<Actor>();
                    game.SelectActor(actor);
                }
            } else
            {
                game.SelectActor(null);
                game.SelectNode(null);
            }
        }
    }
}
