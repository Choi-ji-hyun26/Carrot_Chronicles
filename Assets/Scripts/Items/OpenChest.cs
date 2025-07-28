using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChest : MonoBehaviour
{
    private PlayerMove player;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite opendChest;
    [SerializeField] private GameObject itemPrefab;   //담겨있는 아이템의 프리펩
    private bool isOpened = false;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isOpened = true;
            spriteRenderer.sprite = opendChest;

            if (itemPrefab != null && isOpened == false) //상자가 처음 열렸을때
            {
                //프리펩으로 아이템 만들기
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
            }
            isOpened = true; // 상자가 열렸음 -> 다시 충돌해도 아이템 발생 xx
        }
        
    }
}
