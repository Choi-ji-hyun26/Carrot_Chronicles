using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaMove : EnemyBase
{
    protected CircleCollider2D attackCollider;
    protected float defaultColliderX; // attack collider 초기 x 위치 저장

    protected override void Awake()
    {
        base.Awake();
        attackCollider = GetComponent<CircleCollider2D>();
        defaultColliderX = Mathf.Abs(attackCollider.offset.x);
        UpdateColliderPositionX();
    }

    private void UpdateColliderPositionX() // attack collider x 포지션 변경
    {
        Vector2 offset = attackCollider.offset;

        // flipX 여부에 따라 부호 반전
        offset.x = defaultColliderX * (spriteRenderer.flipX ? 1f : -1f); // flipX true(왼쪽방향) 1, false -1

        attackCollider.offset = offset;
    }
    public void Enbox() // public : 유니티 엔진 애니메이션에서 호출
    {
        attackCollider.enabled = true;
    }
    public void Debox() // public : 유니티 엔진 애니메이션에서 호출
    {
        attackCollider.enabled = false;
    }
}
