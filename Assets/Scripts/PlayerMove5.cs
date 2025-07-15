using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
/*
    이중점프 해결
*/
public class PlayerMove5 : MonoBehaviour
{
    public GameManager gameManager;
    public Transform groundCheck; // 발바닥 기준 위치
    public float maxSpeed = 6;

    // code about jump
    public float firstJumpForce = 20f;
    public float doubleJumpForce = 16f;
    int jumpCount = 0; // 이단점프 위한 count 변수
    int maxJumpCount = 2;
    public bool isJumping = false;

    //new code about ladder
    public bool onLadder;
    public float climbSpeed = 6;
    private float climbDirection;
    //private float gravityStore; //상황에 따라 중력을 다르게 하고 싶다면
    public bool isClimbing = false;
    int playerLayer;
    int platformLayer;


    //지면에 있는지 체크
    bool isGrounded = false; // 땅에 닿아있는지
    public float groundCheckRadius = 0.2f; //for isGrouded
    public LayerMask groundLayer; //for isGrounded

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    Animator animator;

    AudioSource audioSource;

    // new code for ladder
    public float ladderTopY;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() // 단발적인 Key 입력
    {  
        playerLayer = LayerMask.NameToLayer("Player");
        platformLayer = LayerMask.NameToLayer("Platform");

        // Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        // Direction Sprite
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation 
        if(Mathf.Abs(rigid.velocity.x) < 0.3) // 단위가 0이다 -> 멈춤
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

        // 비탈길 중간에 멈출 시 미끄럼 방지
        if(Input.GetAxis("Horizontal") == 0) // idle 상태일때
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else // 방향키 누를 때
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;    

        // 사다리 
        float vInput = Input.GetAxisRaw("Vertical");

        if(onLadder)  // 1. 사다리 접촉 중
        {
            if(Mathf.Abs(vInput) > 0) // 사다리 타기 시작 조건
                EnterLadderMode();
            if(Input.GetButtonDown("Jump"))
                ExitLadderMode();
        }
        if(isClimbing)  // 2. 사다리 상태일 때 입력 처리
        {
            if(Input.GetButtonDown("Jump")) // 점프 탈출
                ExitLadderMode();
            else if (!onLadder) // 사다리에서 떨어졌을 떄 자동 탈출
                ExitLadderMode();
        }

        // Jump
        if(Input.GetButtonDown("Jump"))
        {  
            if(jumpCount < maxJumpCount)
            {
                isJumping = true;
                animator.SetBool("isJumping",true);
                animator.SetBool("isClimbing", false);
            }
            //PlaySound("JUMP"); 
        }            
    }
    
    void FixedUpdate() // 지속적인 key 입력
    {
        // 바닥 체크
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
        if(rigid.velocity.x > maxSpeed) //velocity: 리지드바디의 현 속도 // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); //y 축 속도는 그대로 0을 넣어ㅡ면 안됨
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platform
        if(rigid.velocity.y < 0){ //착지할때도 ray를 그리지 않게하기 위함
            Debug.DrawRay(groundCheck.position, Vector3.down * 0.3f, new Color(1,0,0));
            RaycastHit2D rayHit = Physics2D.Raycast(groundCheck.position, Vector3.down, 0.3f, LayerMask.GetMask("Platform"));// 물리기반, 이 레이어에 해당하는 것만 스캔할 것
            // rayHit : 빔을 쏘고 오브젝트에 대한 정보
            if(rayHit.collider != null) {// 빔을 맞았을 때
                if(rayHit.distance < 0.2f) //잘 밀착했는지
                    animator.SetBool("isJumping", false);
            }
        }

        if (isClimbing)
        { 
            rigid.gravityScale = 0f;

            climbDirection = climbSpeed * Input.GetAxisRaw("Vertical");
            rigid.velocity = new Vector2(rigid.velocity.x, climbDirection);    
        }
        else if (!isClimbing || isJumping)
        {
            rigid.gravityScale = 4f;
        }

        if(isJumping)
        {
            //rigid.gravityScale = 4f; // 중력 복구

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

    void EnterLadderMode()
    {
        isClimbing = true;                   // 사다리 상태 활성화
        rigid.velocity = Vector2.zero;          // 기존 속도 제거 (특히 점프 후)
        animator.SetBool("isJumping", false);
        animator.SetBool("isClimbing", true); // 클라이밍 애니메이션 시작 (선택)
        // 사다리 상태에서는 플랫폼과의 충돌을 강제로 켬
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);

    }
    void ExitLadderMode()
    {
        isClimbing = false;      // 사다리 상태 비활성화
        animator.SetBool("isClimbing", false); // 기본 상태로 전환 (선택)
        // 사다리 상태에서는 플랫폼과의 충돌을 강제로 켬
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);

    }



    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (rigid.velocity.y < -0.01f && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            } else {
                OnDamaged(collision.transform.position);
            }
        }
        // if (collision.gameObject.tag == "Platform"){
        //     isJumping = false;
        // }
    }

   void OnTriggerEnter2D(Collider2D collision) {
        // Item : Carrot, Star
        if (collision.gameObject.tag == "Item"){
            //health
            bool isCarrot = collision.gameObject.name.Contains("Carrot");
            if(isCarrot)
                gameManager.HealthUp();
            // point
            bool isStar = collision.gameObject.name.Contains("Star");
            if(isStar)
                gameManager.stagePoint += 1;
            //Deactive Item
            collision.gameObject.SetActive(false);

            // Sound
            //PlaySound("ITEM");

        }
        // 포탈 : 피니시 구간
        else if (collision.gameObject.tag == "Finish"){ 
            //Next Stage
            gameManager.NextStage();

            //Sound
            //PlaySound("FINISH"); 
        }
        else if (collision.gameObject.tag == "Ladders"){
            onLadder = true;
        }

    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ladders"){
            onLadder = false;
        }
    }

    void OnAttack(Transform enemy)
    {
        //point
        //gameManager.stagePoint += 100;
        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        //Sound
        //PlaySound("ATTACK"); 
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();

        //Change Layer (Immortal Active)
        gameObject.layer = 11; // PlayerDamaged의 Layer 번호
        spriteRenderer.color = new Color(1,1,1,0.4f); //0.4 : 투명도

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //왼쪽에서 맞으면 왼쪽으로, 오른쪽에서 맞으면 오른쪽으로 튕기기
        rigid.AddForce(new Vector2(dirc,1) * 10,ForceMode2D.Impulse);
    
        // Animation
        animator.SetTrigger("doDamaged");

        //Sound
        //PlaySound("DAMAGED");

        Invoke("OffDamaged", 3); // 무적시간 3초
    }

    void OffDamaged()
    {
        gameObject.layer = 10; //Player의 Layer 번호
        spriteRenderer.color = new Color(1,1,1,1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1,1,1,0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Sound
        //PlaySound("DIE"); 
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
