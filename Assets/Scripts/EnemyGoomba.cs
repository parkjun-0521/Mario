using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGoomba : Enemy, IEnemy 
{
    public LayerMask detectionLayer; // Inspector���� ����: Ground�� Enemy ���̾� ����

    void FixedUpdate() {
        if (isDead) 
            return;

        EnemyMove();
    }

    public void EnemyMove() {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.55f, detectionLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.55f, detectionLayer);


        // �浹�� �ڽŰ� �Ͼ���� Ȯ��
        if (hitLeft.collider != null || hitRight.collider != null) {
            isTurn = !isTurn; // ���� ��ȯ ����
        }

        if (!isTurn) {
            transform.Translate(Vector3.left * speed * Time.deltaTime);            
        }
        else {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }


    void OnCollisionEnter2D( Collision2D collision ) {
        if (collision.gameObject.CompareTag("Player")) {
            if (player.position.y > transform.position.y) {
                Enemydead();
            }
            else if (GameManager.Instance.player.GetComponent<Player>().isStar) {
                EnemyStarDead();
            }
        }
        else if (collision.gameObject.CompareTag("FireBall")) {
            EnemyStarDead();
        }
        else if (collision.gameObject.CompareTag("DeadZoon")) {
            gameObject.SetActive(false);
        }
    }

    public override void Enemydead() {
        isDead = true;
        circleCollider2D.enabled = false;
        transform.position -= new Vector3(0, 0.24f);
        rigid.velocity = Vector3.zero;
        rigid.gravityScale = 0;
        animator.SetTrigger("isDead");
        GameManager.Instance.score += this.score;
        StartCoroutine(DestroyGoomba());
    }

    public override void EnemyStarDead() {
        isDead = true;
        rigid.velocity = Vector3.zero;
        animator.enabled = false;
        rigid.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
        transform.Rotate(new Vector3(0f, 0f, 180f));
        circleCollider2D.enabled = false;
        GameManager.Instance.score += this.score;
        StartCoroutine(DestroyGoomba());
    }
    IEnumerator DestroyGoomba() {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
