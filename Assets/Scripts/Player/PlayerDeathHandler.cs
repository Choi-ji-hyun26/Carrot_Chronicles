using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    PlayerMove player;
    // Rigidbody2D rigid;
    // SpriteRenderer spriteRenderer;
    // BoxCollider2D boxCollider;

    void Awake()
    {
        player = GetComponent<PlayerMove>();
        // rigid = GetComponent<Rigidbody2D>();
        // spriteRenderer = GetComponent<SpriteRenderer>();
        // boxCollider = GetComponent<BoxCollider2D>();
    }
    public void OnDie()
    {
        //Sprite Alpha
        player.spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        player.spriteRenderer.flipY = true;
        //Collider Disable
        player.boxCollider.enabled = false;
        //Die Effect Jump
        player.rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Sound
        SoundManager.Instance.PlaySound("DIE");
    }
}
