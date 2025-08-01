using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected BoxCollider2D boxCollider;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void OnDamaged()  //PlayerAttackHandler 에서 호출
    {
        boxCollider.enabled = false;
        rigid.simulated = false;

        animator.Play("Enemy_Death");
        //Destroy
        Invoke("DeActive",0.4f);
    }

    private void DeActive()
    {
        gameObject.SetActive(false);
    }
}
