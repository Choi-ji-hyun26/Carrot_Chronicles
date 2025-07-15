using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    public Text scoreText;
    void Start()
    {
        int score = GameManager.Instance.totalPoint + GameManager.Instance.stagePoint;
        scoreText.text = "SCORE " + score.ToString();
    }

    public void OnRestartButtonClick()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager is null!");
            return;
        }
        GameManager.Instance.Restart();
    }
}
