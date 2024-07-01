using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int score;
    void OnEnable() {
        gameObject.SetActive(true);
    }

    public void CoinDestroy() {
        GameManager.Instance.score += this.score;
        GameManager.Instance.coinScore += 1;
        gameObject.SetActive(false);
    }
}
