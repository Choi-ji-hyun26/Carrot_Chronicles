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
    // public : Player 관련 스크립트
    public Rigidbody2D rigid { get; private set; } 
    public SpriteRenderer spriteRenderer { get; private set; }
    public BoxCollider2D boxCollider { get; private set; }
    public Animator animator { get; private set; }

    [SerializeField] private float maxSpeed = 6;

    // jump
    [SerializeField] private float firstJumpForce = 20f;
    [SerializeField] private float doubleJumpForce = 16f;
    private int jumpCount = 0; // 이단점프 위한 count 변수
    private int maxJumpCount = 2;
    [SerializeField] private bool isJumping = false;

    // ladder
    [SerializeField] private float climbSpeed = 6;
    private float verticalInput;
    [SerializeField] private float defaultGravity = 4f;

    //isGronded : 사다리, 점프 // RayCast
    [SerializeField] private bool isGrounded = false; // 땅에 닿아있는지, 현재 프레임의 접지 상태
    [SerializeField] private ContactFilter2D groundFilter; // Layer : Platform
    [SerializeField] private float groundCheckDistance = 0.1f;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5]; // 결과 저장용 배열

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update() // 단발적인 Key 입력
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

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (IsTouchingLadder(boxCollider))
            {
                return; // 사다리 접촉 중엔 점프 안됨
            }
            if (jumpCount < maxJumpCount)
            {
                isJumping = true;
                animator.SetBool("isJumping", true);
                animator.SetBool("isWalking", false);
            }
            SoundManager.Instance.PlaySound("JUMP");
        }
        //사다리 
        if (IsTouchingLadder(boxCollider))
        {
            verticalInput = Input.GetAxisRaw("Vertical");

            animator.SetBool("climbStill", true);
            animator.SetBool("isJumping", false);

            if (Mathf.Abs(verticalInput) > Mathf.Epsilon)
            {
                animator.SetBool("isClimbing", true);
            }
            else
            {
                animator.SetBool("isClimbing", false);
            }
        }
        else
        {
            animator.SetBool("isClimbing", false);
            animator.SetBool("climbStill", false);
        }
    }

    private void FixedUpdate() // 지속적인 key 입력
    {
        //Landing Platform
        CheckGrounded();

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

        
        if (IsTouchingLadder(boxCollider))
        {
            // 중력 제거 & 위/아래 이동
            rigid.gravityScale = 0f;
            Vector2 playerClimbVelocity = new Vector2(rigid.velocity.x, verticalInput * climbSpeed);
            rigid.velocity = playerClimbVelocity;
        }
        else
        {
            // 사다리에서 벗어났으면 중력 복원
            rigid.gravityScale = defaultGravity; // 예: 4f 또는 선언한 값
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

    //Check to see if player is touching ladder
    private bool IsTouchingLadder(BoxCollider2D player)
    {
        int ladder = LayerMask.GetMask("Ladders");

        return player.IsTouchingLayers(ladder);
    }
    
    private void CheckGrounded()
    {
        isGrounded = false;
        if (rigid.velocity.y < 0) // 낙하 중일 때만 검사
        {
            int hitCount = rigid.Cast(Vector2.down, groundFilter, groundHits, groundCheckDistance);

            if (hitCount > 0)
            {
                if (groundHits[0].distance < groundCheckDistance)
                {
                    isGrounded = true;
                    animator.SetBool("isJumping", false);
                }
            }
        }
    }
    public void VelocityZero() // public : GameManager 호출
    {
        rigid.velocity = Vector2.zero;
    }
}