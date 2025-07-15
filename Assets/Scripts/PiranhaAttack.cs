using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaAttack : MonoBehaviour
{
    BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void enboxCollider()
    {
        boxCollider.enabled = true;
    }
    public void deboxCollider()
    {
        boxCollider.enabled = false;
    }
}
