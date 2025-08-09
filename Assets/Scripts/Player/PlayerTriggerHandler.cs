using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 당근, 별  아이템
        if (collision.TryGetComponent<ICollectible>(out var item))
        {
            item.OnCollected(gameObject);
            return;
        }
        else if (collision.CompareTag("Finish"))
        {
            //Next Stage
            StartCoroutine(GameManager.Instance.NextStage());

            //Sound
            SoundManager.Instance.PlaySound("FINISH");
        }
        //Chest는 Items - ChestItems.cs 에서 처리
    }
}
