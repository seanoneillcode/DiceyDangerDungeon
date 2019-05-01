using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnce : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, this.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}
