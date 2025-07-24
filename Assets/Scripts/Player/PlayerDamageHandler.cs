using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    PlayerMove player;
    // Rigidbody2D rigid;
    // SpriteRenderer spriteRenderer;
    // Animator animator;

    void Awake()
    {
        player = GetComponent<PlayerMove>();
        // rigid = GetComponent<Rigidbody2D>();
        // spriteRenderer = GetComponent<SpriteRenderer>();
        // animator = GetComponent<Animator>();
    }
    public void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        Stats.instance.HealthDown();

        //Change Layer (Immortal Active)
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged"); 
        player.spriteRenderer.color = new Color(1, 1, 1, 0.4f); //0.4 : 투명도

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //왼쪽에서 맞으면 왼쪽으로, 오른쪽에서 맞으면 오른쪽으로 튕기기
        player.rigid.AddForce(new Vector2(dirc, 1) * 10, ForceMode2D.Impulse);

        // Animation
        player.animator.SetTrigger("doDamaged");

        //Sound
        SoundManager.Instance.PlaySound("DAMAGED");

        Invoke("OffDamaged", 3); // 무적시간 3초
    }

        void OffDamaged()
    {
        if (GetComponentInParent<PlayerChestHandler>().isUnBeatTime) // 데미지 입은 상태에 피버타임 겹치는 경우 layer 변동이 없도록
            return;
        gameObject.layer = LayerMask.NameToLayer("Player");
        player.spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
