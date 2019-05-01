using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }
}
