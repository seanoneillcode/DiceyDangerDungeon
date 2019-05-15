using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject plantPrefab;
    public GameObject portalPrefab;

    public void Explode(Vector3 position)
    {
        Instantiate(explosionPrefab, position, Quaternion.identity);
    }

    public void PickupPlant(Vector3 position)
    {
        Instantiate(plantPrefab, position, Quaternion.identity);
    }

    internal void PortalExplosion(Vector3 position)
    {
        Instantiate(portalPrefab, position, Quaternion.identity);
    }
}
