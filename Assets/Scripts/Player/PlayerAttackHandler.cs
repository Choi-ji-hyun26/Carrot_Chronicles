using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (GetComponentInParent<PlayerChestHandler>().isUnBeatTime)
            {
                OnAttack(collision.transform);
            }
            else if (rigid.velocity.y < -0.01f && transform.position.y > collision.transform.position.y)
            { // 점프 공격 또는 무적상태일때
                OnAttack(collision.transform);
            }
            else
            {
                GetComponentInParent<PlayerDamageHandler>().OnDamaged(collision.transform.position);
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        if (enemy == null)
            return;

        //Reaction Force
        if (!GetComponentInParent<PlayerChestHandler>().isUnBeatTime) // 무적이 아닌 기본 상태에서 공격은 점프! 무적은 아무 움직임 x
            rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        if (enemyMove == null)
            return;
        enemyMove.OnDamaged();

        //Sound
        SoundManager.Instance.PlaySound("ATTACK");
    }
}
