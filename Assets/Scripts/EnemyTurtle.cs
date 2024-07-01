using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurtle : Enemy, IEnemy
{
    public GameObject shellPrefab;
    public Sprite deadImage;
    public LayerMask detectionLayer; // Inspector에서 설정: Ground와 Enemy 레이어 포함
    void FixedUpdate() {
        if (isDead)
            return;

        EnemyMove();
    }

    public void EnemyMove() {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.55f, detectionLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.55f, detectionLayer);

        // 충돌이 자신과 일어났는지 확인
        if ((hitLeft.collider != null && hitLeft.collider != circleCollider2D) ||
            (hitRight.collider != null && hitRight.collider != circleCollider2D)) {
            isTurn = !isTurn;
        }

        if (!isTurn) {
            spriteRenderer.flipX = false;
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else {
            spriteRenderer.flipX = true;
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
        ShellDrop(shellPrefab);
        GameManager.Instance.score += this.score;
        gameObject.SetActive(false);
    }

  
    public override void EnemyStarDead() {
        isDead = true;
        rigid.velocity = Vector3.zero;
        animator.enabled = false;
        spriteRenderer.sprite = deadImage;
        rigid.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
        transform.Rotate(new Vector3(0f, 0f, 180f));
        circleCollider2D.enabled = false;
        GameManager.Instance.score += this.score;
        StartCoroutine(DestroyTurtle());
    }

    IEnumerator DestroyTurtle() {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public override void ShellDrop( GameObject shell ) {
        GameObject shellObj = GameManager.Instance.pooling.GetObject(10);
        shellObj.transform.position = transform.position;
        shellObj.transform.rotation = Quaternion.identity;
    }
}
