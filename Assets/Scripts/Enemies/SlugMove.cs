//using System;
using System.Collections;
using UnityEngine;

public class SlugMove : EnemyBase
{
    // 상수화
    private const int MOVE_LEFT = -1;
    private const int MOVE_IDLE = 0;
    private const int MOVE_RIGHT = 1;

    private int nextMove;
    private Coroutine thinkCoroutine;

    protected override void Awake()
    {
        base.Awake();
        thinkCoroutine = StartCoroutine(ThinkRoutine(2f)); // 초기 딜레이 2초
    }

    private void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove,rigid.velocity.y);
        
        //Platform Check
        Vector2 frontVector = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVector, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVector, Vector3.down, 1, LayerMask.GetMask("Platform"));// 물리기반, 이 레이어에 해당하는 것만 스캔할 것
        if(rayHit.collider == null) 
        {
            Turn();
        }
    }

    private IEnumerator ThinkRoutine(float initialDelay = 0f)
    {
        // 초기 딜레이
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            nextMove = Random.Range(MOVE_LEFT, MOVE_RIGHT+1); // -1, 0, 1 중 선택

            // Sprite 방향 전환
            if (nextMove != MOVE_IDLE)
                spriteRenderer.flipX = nextMove == 1;
            // 다음 Think까지 딜레이
            float nextThinkTime = Random.Range(2f, 4f);
            yield return new WaitForSeconds(nextThinkTime);
        }
    }
    private void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        // Coroutine 재시작 : 기존 코루틴 정지 후 5초 뒤 재시작
        if (thinkCoroutine != null)
            StopCoroutine(thinkCoroutine);

        thinkCoroutine = StartCoroutine(ThinkRoutine(4f));
    }
}
