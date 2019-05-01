using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{

    public Material highlightMouseMaterial;
    public Material highlightSelectMaterial;
    private GameObject floorHighlight;
    private GameObject floorMouseHighlight;

    private Game game;
    private Actor lastHighlightedActor;
    private Actor lastSelectedActor;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
        floorHighlight = transform.GetChild(0).gameObject;
        floorMouseHighlight = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastHighlightedActor != null)
        {
            lastHighlightedActor.Highlight(null);
            lastHighlightedActor = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Actors", "Nodes")))
        {
            Actor actor = hit.transform.gameObject.GetComponent<Actor>();
            if (actor != null)
            {
                actor.Highlight(highlightMouseMaterial);
                lastHighlightedActor = actor;
            }
            Node node = hit.transform.gameObject.GetComponent<Node>();
            if (node != null && node != game.selectedNode)
            {
                floorMouseHighlight.SetActive(true);
                floorMouseHighlight.transform.position = node.gameObject.transform.position;
            } else
            {
                floorMouseHighlight.SetActive(false);
            }
        } else
        {
            floorMouseHighlight.SetActive(false);
        }

        if (game.selectedActor != null)
        {
            game.selectedActor.Highlight(highlightSelectMaterial);
            lastSelectedActor = game.selectedActor;
        } else
        {
            if (lastSelectedActor != null)
            {
                lastSelectedActor.Highlight(null);
                lastSelectedActor = null;
            }
        }

        HighlightFloors();
    }

    private void HighlightFloors()
    {
        if (game.selectedNode != null)
        {
            floorHighlight.SetActive(true);
            floorHighlight.transform.position = game.selectedNode.gameObject.transform.position;
        }
        else
        {
            floorHighlight.SetActive(false);
        }
    }

    private void RemoveHighlight(Actor actor)
    {
        actor.Highlight(null);
    }
}
