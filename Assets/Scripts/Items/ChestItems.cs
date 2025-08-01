using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public enum ItemType
{
    clover,
    scroll,
}

public class ChestItems : MonoBehaviour
{
    [SerializeField] private GameObject miniMapCamera;
    public ItemType type;
    //public int arrangeId = 0;

    private Rigidbody2D itemBody;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" )
        {
            var chestHandler = collision.GetComponent<PlayerChestHandler>();
            if (chestHandler == null) return;

            if (type == ItemType.clover)
            {
                chestHandler.isClover = true;
                //Debug.Log("Clover");
            }
            else if (type == ItemType.scroll)
            {
                miniMapCamera.SetActive(true);
                //Debug.Log("Scroll");
            }

            //++++ 아이템 획득 연출 ++++
            //충돌 판정 비활성
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            //아이템의 Rigidbody2D가져오기
            Rigidbody2D itemBody = GetComponent<Rigidbody2D>();
            //중력 젹용
            if (itemBody != null)
            {
                itemBody.gravityScale = 2.5f;
                //위로 튀어오르는 연출
                itemBody.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("Rigidbody2D가 없음!");
            }
            //0.5초 뒤에 제거
            Destroy(gameObject, 1f);
        }
    }
}
