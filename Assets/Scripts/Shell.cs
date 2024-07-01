using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public int speed;
    bool isMovingLeft;
    bool isHit;
    public bool isThrow;

    float startTiem;
    float maxTime = 0.5f;

    public int score;

    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
    }

    void OnEnable() {
        capsule.enabled = true;
    }

    void Start() {
        startTiem = Time.time;
    }

    void Update() {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.55f, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.55f, LayerMask.GetMask("Ground"));

        // 벽에 부딪히면 방향 전환
        if (hitLeft.collider != null || hitRight.collider != null) {
            isMovingLeft = !isMovingLeft; // 방향 전환
        }

        // 방향에 따라 이동
        if (isHit) {
            isThrow = true;
            if (isMovingLeft) {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            else {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter2D( Collision2D collision ) {
        if(startTiem + Time.time < maxTime) { return; }

        if (collision.gameObject.CompareTag("Player")) {
            GameManager.Instance.score += this.score;
            isHit = true;
            isMovingLeft = collision.gameObject.transform.position.x > gameObject.transform.position.x;
        }
        else if (collision.gameObject.CompareTag("FireBall")) {
            capsule.enabled = false;
            rigid.velocity = Vector3.zero;
            rigid.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
            transform.Rotate(new Vector3(0f, 0f, 180f));
            GameManager.Instance.score += this.score;
            StartCoroutine(DestroyTurtle());
        }
        else if(collision.gameObject.layer == 11) {
            gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Player") && isThrow) {     
            Player playerLogic = collision.gameObject.GetComponent<Player>();
            if (playerLogic.hp > 0) {
                playerLogic.hp -= 1;
                if (playerLogic.hp == 0) {
                    playerLogic.MarioChange(0, 0.5f, 0.25f, 0.25f, 0.25f);    // 작은 마리오 
                }
                else if (playerLogic.hp == 1) {
                    playerLogic.MarioChange(1, 1f, 0.5f, 0.25f, 0.75f);     // 큰 마리오
                }
            }
            else {
                playerLogic.MarioDead();
            }
        }
        else if (collision.gameObject.CompareTag("Enemy") && isThrow) {
            collision.gameObject.GetComponent<Enemy>().EnemyStarDead();
        }
    }

    IEnumerator DestroyTurtle() {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
