using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private PlayerMove player;

    private void Awake()
    {
        player = GetComponent<PlayerMove>();
    }
    public void OnDie() // public : PlayerDeathHandler 호출
    {
        //Sprite Alpha
        player.spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        player.spriteRenderer.flipY = true;
        //Collider Disable
        player.boxCollider.enabled = false;
        //Die Effect Jump
        player.rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 카메라 따라가기 멈추기
        FindObjectOfType<CameraMove>().StopFollowing();
        //Sound
        SoundManager.Instance.PlaySound("DIE");
    }
}
