using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void EnemyMove();

    void Enemydead();
    void EnemyStarDead();
}

public class Enemy : MonoBehaviour {
    public float speed;

    public bool isDead;
    public bool isTurn;
    public int score;

    [HideInInspector]
    public CircleCollider2D circleCollider2D;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody2D rigid;
    [HideInInspector]
    public Transform player;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    public virtual void ShellDrop(GameObject shell ) { return; }
    public virtual void Enemydead() { return; }
    public virtual void EnemyStarDead() { return; }

    void Awake() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable() {
        isDead = false;
        isTurn = false;
        circleCollider2D.enabled = true;
        animator.enabled = true;
        rigid.velocity = Vector3.zero;
        rigid.gravityScale = 1;
        transform.Rotate(new Vector3(0f, 0f, 0f));
    }

    void Start() {
        player = GameManager.Instance.player.GetComponent<Transform>();
    }
}
