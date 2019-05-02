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
    private Player lastHighlightedPlayer;
    private Player lastSelectedPlayer;

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
        if (lastHighlightedPlayer != null)
        {
            lastHighlightedPlayer.Highlight(null);
            lastHighlightedPlayer = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Actors", "Nodes")))
        {
            Player player = hit.transform.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.Highlight(highlightMouseMaterial);
                lastHighlightedPlayer = player;
            }
            
            Node node = hit.transform.gameObject.GetComponent<Node>();
            if (player == null && node == null)
            {
                Actor actor = hit.transform.gameObject.GetComponent<Actor>();
                if (actor != null && actor.node != null)
                {
                    node = actor.node;
                }
            }
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

        if (game.selectedPlayer != null)
        {
            game.selectedPlayer.Highlight(highlightSelectMaterial);
            lastSelectedPlayer = game.selectedPlayer;
        } else
        {
            if (lastSelectedPlayer != null)
            {
                lastSelectedPlayer.Highlight(null);
                lastSelectedPlayer = null;
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
}
