using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomItem : MonoBehaviour
{
    public float speed;

    bool isTurn;
    public int score;
    void OnEnable() {
        // �����ϴ� �ִϸ��̼� �߰�    
        Debug.Log("���� �ִϸ��̼� �����ߵ� ");
    }

    void Update() {
        EnemyMove();
    }

    void EnemyMove() {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.55f, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.55f, LayerMask.GetMask("Ground"));
        if (hitLeft.collider != null || hitRight.collider != null)
            isTurn = !isTurn;

        if (!isTurn) {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else {
            transform.Translate(Vector3.left * speed * Time.deltaTime);      
        }
    }

    void OnCollisionEnter2D( Collision2D collision ) {
        if (collision.gameObject.CompareTag("Player")) {
            GameManager.Instance.score += this.score;
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("DeadZoon")) {
            gameObject.SetActive(false);
        }
    }
}
