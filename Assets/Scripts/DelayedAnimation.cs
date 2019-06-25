using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAnimation : MonoBehaviour
{

    public string animationName;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator.Play(animationName, 0, Random.Range(0.5f, 1.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
