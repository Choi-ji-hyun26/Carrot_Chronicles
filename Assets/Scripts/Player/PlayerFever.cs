using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerFever : MonoBehaviour
{
    public Text feverText;

    // 무적 타임, playermove에서 호출
    public void ActivateFeverMode()
    {
        StartCoroutine(FeverCoroutine());
    }
    private IEnumerator FeverCoroutine()
    {
        int feverDuration = 7;

        feverText.gameObject.SetActive(true);

        for (int i = feverDuration; i > 0; i--)
        {
            feverText.text = $"FEVER {i}";
            yield return new WaitForSeconds(1f);
        }

        feverText.text = "";
        feverText.gameObject.SetActive(false);
    }
}
