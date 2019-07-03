using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    private Animator animator;
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        game = FindObjectOfType<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && Vector3.Distance(target.position, transform.position) > 0.9f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * 2);
            animator.SetBool("isRunning", true);
            animator.SetBool("isAttacking", false);
        } else
        {
            animator.SetBool("isRunning", false);
            if (game.actionedNode != null && game.actionedNode.risk > 0)
            {
                if (game.HasDiceRoll())
                {
                    animator.SetBool("isAttacking", true);
                }
                else
                {
                    animator.SetBool("isAttacking", false);
                }
            }
        }
    }

    internal void SetFollow(Transform transform)
    {
        float xoff = (UnityEngine.Random.Range(0, 2) - 1) * 0.5f;
        float zoff = (UnityEngine.Random.Range(0, 2) - 1) * 0.5f;
        offset = new Vector3(xoff, transform.position.y, zoff);
        target = transform;
    }
}
