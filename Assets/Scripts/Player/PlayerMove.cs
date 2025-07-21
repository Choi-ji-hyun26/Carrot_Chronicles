using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    public Transform groundCheck; // 발바닥 기준 위치
    public float maxSpeed = 6;

    //new code about ladder
    public bool onLadder;
    public float climbSpeed = 6;
    private float climbDirection;
    //private float gravityStore; //상황에 따라 중력을 다르게 하고 싶다면
    public bool canClimbing = false;
    public bool isClimbing = false;

    //지면에 있는지 체크
    public bool isGrounded = false; // 땅에 닿아있는지
    public float groundCheckRadius = 0.2f; //for isGrouded
    public LayerMask groundLayer; //for isGrounded

    // 무적 타임
    public bool isUnBeatTime = false;

    // 보물상자 아이템
    public bool isClover;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update() // 단발적인 Key 입력
    {
        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        // Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation 
        if (Mathf.Abs(rigid.velocity.x) < 0.3) // 단위가 0이다 -> 멈춤
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

        // 비탈길 중간에 멈출 시 미끄럼 방지
        if (Input.GetAxis("Horizontal") == 0) // idle 상태일때
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else // 방향키 누를 때
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (isGrounded) //5.23 사다리 다 내려갔음에도 오르는 애니메이션 유지되길래 추가함 -> 해결됨
        {
            animator.SetBool("isClimbing", false);
        }

        // // Jump

        if (onLadder && Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f) //사다리와 겹치고 위아래 키 조작이 있어야만 
        {
            isClimbing = true;

            animator.SetBool("isClimbing", true);
            animator.SetBool("isJumping", false); // 사다리에서 점프로 오르기 할 때 애니메이션 변경위해 

        }
        else if (!onLadder)
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
        }
    }

    void FixedUpdate() // 지속적인 key 입력
    {
        // // 바닥 체크
         isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 땅에 닿았으면 점프 카운트 초기화
        // if (isGrounded)
        // {
        //     jumpCount = 0;
        // }

        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //velocity: 리지드바디의 현 속도 // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); //y 축 속도는 그대로 0을 넣어ㅡ면 안됨
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platform

        if (isClimbing)
        {
            rigid.gravityScale = 0f;

            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
            {
                climbDirection = climbSpeed * Input.GetAxisRaw("Vertical");
                rigid.velocity = new Vector2(rigid.velocity.x, climbDirection);
            }
            else
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0); // 움직임 없음
            }

            Debug.Log("Gravity : " + rigid.gravityScale);
        }
        else
        {
            rigid.gravityScale = 4f;
            Debug.Log("Gravity : " + rigid.gravityScale);
        }

        //JUMP
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (isUnBeatTime == true)
            {
                OnAttack(collision.transform);
            }
            else if (rigid.velocity.y < -0.01f && transform.position.y > collision.transform.position.y)
            { // 점프 공격 또는 무적상태일때
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 당근, 별  아이템
        if (collision.TryGetComponent<ICollectible>(out var item))
        {
            item.OnCollected(gameObject);
            return;
        }

        // 보물상자
        if (collision.gameObject.tag == "Chest")
        {
            if (isClover) // public bool 변수를 만들어서,, ChestItem에서 이 변수만 접근하게?
            {
                gameObject.layer = 12; // 무적 상태 layer

                isUnBeatTime = true;
                StartCoroutine(UnBeatTime());
                gameObject.GetComponentInParent<PlayerFever>().ActivateFeverMode();

                SoundManager.Instance.PlaySound("ITEM");
            }
        }
        // 포탈 : 피니시 구간
        else if (collision.gameObject.tag == "Finish")
        {
            //Next Stage
            GameManager.Instance.NextStage();

            //Sound
            SoundManager.Instance.PlaySound("FINISH");
        }

    }

    private IEnumerator UnBeatTime()
    {
        int countTime = 0;

        while (countTime < 35) // 35 * 0.2 = 7초
        {
            if (countTime % 2 == 0)
                spriteRenderer.color = new Color32(255, 255, 255, 90); //반투명
            else
                spriteRenderer.color = new Color32(255, 255, 255, 180); // 좀더 진하게

            yield return new WaitForSeconds(0.2f); // 깜빡임, 35 * 0.2 = 7초

            countTime++;
        }

        gameObject.layer = 10; // 원래 상태 layer
        spriteRenderer.color = new Color32(255, 255, 255, 255);
        isUnBeatTime = false;
        isClover = false;

        yield return null;
    }
    void OnAttack(Transform enemy)
    {
        if (enemy == null)
            return;

        //Reaction Force
        if (!isUnBeatTime) // 무적이 아닌 기본 상태에서 공격은 점프! 무적은 아무 움직임 x
            rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        if (enemyMove == null)
            return;
        enemyMove.OnDamaged();

        //Sound
        SoundManager.Instance.PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        Stats.instance.HealthDown();

        //Change Layer (Immortal Active)
        gameObject.layer = 11; // PlayerDamaged의 Layer 번호
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //0.4 : 투명도

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //왼쪽에서 맞으면 왼쪽으로, 오른쪽에서 맞으면 오른쪽으로 튕기기
        rigid.AddForce(new Vector2(dirc, 1) * 10, ForceMode2D.Impulse);

        // Animation
        animator.SetTrigger("doDamaged");

        //Sound
        SoundManager.Instance.PlaySound("DAMAGED");

        Invoke("OffDamaged", 3); // 무적시간 3초
    }

    void OffDamaged()
    {
        gameObject.layer = 10; //Player의 Layer 번호
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Sound
        SoundManager.Instance.PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}