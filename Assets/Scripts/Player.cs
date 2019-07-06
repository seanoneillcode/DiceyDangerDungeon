using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Material originalMaterial;
    [HideInInspector] public Vector3 targetPos;

    private Animator animator;
    public SpriteRenderer spriteRenderer;

    public float speed = 2f;
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = GetComponentInChildren<SpriteRenderer>().material;
        targetPos = transform.position;
        animator = GetComponentInChildren<Animator>();
        game = FindObjectOfType<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(targetPos, transform.position);
        if (dist < 0.01f)
        {
            dist = 0;
            transform.position = targetPos;
        }
        if (dist > 0f)
        {
            if (targetPos.z > transform.position.z || targetPos.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
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
            animator.SetBool("isPlayerRunning", true);
            
        } else
        {
            animator.SetBool("isPlayerRunning", false);
            if (game.actionedNode != null && game.actionedNode.risk > 0)
            {
                animator.SetBool("isPlayerFighting", true);
                if (game.didSucceed)
                {
                    animator.SetBool("isPlayerHit", true);
                }
                else
                {
                    animator.SetBool("isPlayerHit", false);
                }
            }
            else
            {
                animator.SetBool("isPlayerHit", false);
                animator.SetBool("isPlayerFighting", false);
            }
            
        }
        if (game.celebrating)
        {
            animator.SetBool("isPlayerCelebrating", true);
        }
        else
        {
            animator.SetBool("isPlayerCelebrating", false);
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
