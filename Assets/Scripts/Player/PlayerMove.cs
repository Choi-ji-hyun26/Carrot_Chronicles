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

    // jump
    public float firstJumpForce = 20f;
    public float doubleJumpForce = 16f;
    int jumpCount = 0; // 이단점프 위한 count 변수
    int maxJumpCount = 2;
    public bool isJumping = false;

    // ladder
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

    public Rigidbody2D rigid { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public BoxCollider2D boxCollider { get; private set; }
    public Animator animator { get; private set; }

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

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < maxJumpCount)
            {
                isJumping = true;
                animator.SetBool("isJumping", true);
                animator.SetBool("isClimbing", false);
            }
            SoundManager.Instance.PlaySound("JUMP");
        }

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
        if (isGrounded)
        {
            jumpCount = 0;
        }

        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //velocity: 리지드바디의 현 속도 // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); //y 축 속도는 그대로 0을 넣어ㅡ면 안됨
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platform
        if (rigid.velocity.y < 0)
        { //착지할때도 ray를 그리지 않게하기 위함
            Debug.DrawRay(groundCheck.position, Vector3.down * 0.3f, new Color(1, 0, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(groundCheck.position, Vector3.down, 0.3f, LayerMask.GetMask("Platform"));// 물리기반, 이 레이어에 해당하는 것만 스캔할 것
            // rayHit : 빔을 쏘고 오브젝트에 대한 정보
            if (rayHit.collider != null)
            {// 빔을 맞았을 때
                if (rayHit.distance < 0.2f) //잘 밀착했는지
                    animator.SetBool("isJumping", false);
            }
        }

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
        if (isJumping)
        {
            float appliedForce = (jumpCount == 0) ? firstJumpForce : doubleJumpForce; // 점프 횟수에 따라 높이 다르게 적용

            //수직 속도 초기화해서 점프 높이 일정하게
            if (jumpCount > 0)
                rigid.velocity = new Vector2(rigid.velocity.x, 0);

            rigid.velocity = new Vector2(rigid.velocity.x, appliedForce);
            jumpCount++;
            isJumping = false;

            Debug.Log($"Jump Count: {jumpCount}, Applied Force: {appliedForce}");

        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}