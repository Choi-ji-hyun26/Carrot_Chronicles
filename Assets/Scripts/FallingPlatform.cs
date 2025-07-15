using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.1f; //밟고 나서 떨어지는 시간
    public float respawnDelay = 3f; //다시 생성되는 시간
    public Vector3 resetPositionOffset; // 원래 위에 얼마나 떨어졌는지

    private Rigidbody2D rigid;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isFalling = false;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player" && isFalling) return;
        
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f) // 접촉한 표면이 플레이어의 아래에 있음
            {
                isFalling = true;
                Invoke(nameof(Fall), fallDelay);
                break;
            }
        }
    }

     void Fall()
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        Invoke(nameof(ResetPlatform), respawnDelay);
    }
    void ResetPlatform()
    {
        rigid.bodyType = RigidbodyType2D.Static;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0f;

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        isFalling = false;
    }

}
