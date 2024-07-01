using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] enemySpawn;
    public int enemyNum;
    private bool triggered = false;  // Ʈ���� Ȱ��ȭ ���¸� Ȯ���ϴ� �÷���

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered) {
            triggered = true;  // Ʈ���Ű� Ȱ��ȭ�Ǿ����� ǥ��
            Debug.Log(enemySpawn.Length);
            for (int i = 0; i < enemySpawn.Length; i++) {
                GameObject enemyPrefab = GameManager.Instance.pooling.GetObject(enemyNum);
                enemyPrefab.transform.position = enemySpawn[i].transform.position;
                enemyPrefab.transform.rotation = enemySpawn[i].transform.rotation;
            }
        }
    }
}
