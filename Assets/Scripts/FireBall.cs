using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    public GameObject fireworkParticle;

    int direction = 1; 
    Rigidbody2D rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // ���̾ ���� �� �÷��̾��� ������ Ȯ���ϰ� �ʱ� ������ ����
        Player playerLogic = GameManager.Instance.player.GetComponent<Player>();
        if (playerLogic != null) {
            direction = playerLogic.spriteRenderer.flipX ? -1 : 1; // �÷��̾ ������ ���� -1, �������� ���� 1
        }
    }

    void Start() {     
        rigid.velocity = new Vector2(speed * direction, rigid.velocity.y); // �ʱ� �ӵ� ����
    }

    void Update() {
        //transform.Translate(Vector2.left * speed * Time.deltaTime);
        //rigid.AddForce(Vector2.left * speed * Time.deltaTime, ForceMode2D.Impulse);
        rigid.velocity = new Vector2(speed* direction, rigid.velocity.y);
    }

    void OnCollisionEnter2D( Collision2D collision ) {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Shell")) {
            GameObject particle = GameManager.Instance.pooling.GetObject(1);
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.layer == 11) {
            gameObject.SetActive(false);    
        }
        else if(collision.gameObject.transform.position.y > gameObject.transform.position.y) {
            GameObject particle = GameManager.Instance.pooling.GetObject(1);
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            gameObject.SetActive(false);
        }
    }
}
