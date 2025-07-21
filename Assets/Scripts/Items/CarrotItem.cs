using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotItem : MonoBehaviour, ICollectible
{
    public void OnCollected(GameObject player)
    {
        Stats.instance.HealthUp();
        SoundManager.Instance.PlaySound("ITEM");
        gameObject.SetActive(false);
    }
}
