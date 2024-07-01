using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text coinText;
    public Text stageText;
    public Text timeText;
    private float remainingTime = 400f;
    private bool isTimeStopped = false; 

    void Update() {
        scoreText.text = GameManager.Instance.score.ToString("D6");
        coinText.text = "x"+GameManager.Instance.coinScore.ToString("D2");
        stageText.text = "1-1";
        // remainingTime ���Ҹ� isTimeStopped �÷��׷� ����
        if (remainingTime > 0 && !isTimeStopped) {
            remainingTime -= Time.deltaTime;
        }
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (remainingTime > 0) {
            timeText.text = Mathf.CeilToInt(remainingTime).ToString("D3");
        }
        else {
            timeText.text = "000";
        }
    }

    // �ܺο��� ȣ�� ������ �޼���� �ð� ������ ����
    public void StopTime()
    {
        isTimeStopped = true;
    }

    public void StartTime()
    {
        remainingTime = 400f;
        isTimeStopped = false;
    }
}
