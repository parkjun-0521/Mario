using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] enemySpawn;
    public int enemyNum;
    private bool triggered = false;  // 트리거 활성화 상태를 확인하는 플래그

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered) {
            triggered = true;  // 트리거가 활성화되었음을 표시
            Debug.Log(enemySpawn.Length);
            for (int i = 0; i < enemySpawn.Length; i++) {
                GameObject enemyPrefab = GameManager.Instance.pooling.GetObject(enemyNum);
                enemyPrefab.transform.position = enemySpawn[i].transform.position;
                enemyPrefab.transform.rotation = enemySpawn[i].transform.rotation;
            }
        }
    }
}
