using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime);
        }
    }

    internal void SetFollow(Transform transform)
    {
        float xoff = (UnityEngine.Random.Range(0, 2) - 1) * 0.5f;
        float zoff = (UnityEngine.Random.Range(0, 2) - 1) * 0.5f;
        offset = new Vector3(xoff, 0, zoff);
        target = transform;
    }
}
