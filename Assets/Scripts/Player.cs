using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Material originalMaterial;
    [HideInInspector] public Vector3 targetPos;

    public float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = GetComponentInChildren<SpriteRenderer>().material;
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(targetPos, transform.position);
        if (dist > 0f)
        {
            Vector3 direction = Vector3.Normalize((targetPos - transform.position));
            Vector3 movement = direction * Time.deltaTime * speed;
            if (movement.magnitude >= dist)
            {
                transform.position = targetPos;
            }
            else
            {
                transform.position += movement;
            }
        }
    }

    internal void Highlight(Material highlightMaterial)
    {
        Material refMaterial = highlightMaterial == null ? originalMaterial : highlightMaterial;
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.material = refMaterial;
        }
    }

    public bool IsAtTarget()
    {
        return targetPos.Equals(transform.position);
    }

    internal void Teleport(Vector3 newPos)
    {
        transform.position = newPos;
        targetPos = newPos;
    }
}
