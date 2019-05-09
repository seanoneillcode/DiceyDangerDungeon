using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject plant;
    public bool consumed;

    // Start is called before the first frame update
    void Start()
    {
        consumed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (consumed && plant.activeSelf)
        {
            plant.SetActive(false);
        }
    }

    public void Consume()
    {
        consumed = true;
    }
}
