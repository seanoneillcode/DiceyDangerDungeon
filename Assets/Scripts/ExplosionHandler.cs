using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    public GameObject explosionPrefab;

    public void Explode(Vector3 position)
    {
        Instantiate(explosionPrefab, position, Quaternion.identity);
    }
}
