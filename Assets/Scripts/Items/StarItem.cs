using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour, ICollectible
{
    public void OnCollected(GameObject collector)
    {
        Stats.instance.stagePoint += 1;
        SoundManager.Instance.PlaySound("ITEM");
        gameObject.SetActive(false);
    }
}
