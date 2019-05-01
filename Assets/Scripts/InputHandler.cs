using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
                Transform objectHit = hit.transform;
                Actor actor = hit.transform.gameObject.GetComponent<Actor>();
                if (actor != null)
                {
                    actor.selected = true;
                }
            }
        }
    }
}
