using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaAttack : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void enboxCollider() // PiranhaMove 에서 호출
    {
        boxCollider.enabled = true;
    }
    public void deboxCollider() // PiranhaMove 에서 호출
    {
        boxCollider.enabled = false;
    }
}
