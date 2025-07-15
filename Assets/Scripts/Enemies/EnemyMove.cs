using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class EnemyMove : MonoBehaviour
{
    public Rigidbody2D rigid;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    public int nextMove;
    public PiranhaAttack piranhaAttack;


    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        // Invoke("Think", 5); // 5초뒤에 함수 호출
    }

    public virtual void OnDamaged()
    {
        //피라냐 식물 밀림 현상 관련 코드 new
        //rigid.constraints = RigidbodyConstraints2D.None;
        //Sprite Alpha
        spriteRenderer.color = new Color(1,1,1,0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        rigid.gravityScale = 4f;
        //Destroy
        Invoke("DeActive",5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

    public void enbox()
    {
        if (piranhaAttack != null && piranhaAttack.gameObject.activeInHierarchy)
        {
            piranhaAttack.enboxCollider();
        }    
    }
    public void debox()
    {
        if (piranhaAttack != null && piranhaAttack.gameObject.activeInHierarchy)
        {
            piranhaAttack.deboxCollider();
        }
    }
}
