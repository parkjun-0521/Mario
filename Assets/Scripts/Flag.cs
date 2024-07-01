using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public Transform ball;
    Animator animator;

    public int score;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D( Collision2D collision ) {
        if (collision.gameObject.CompareTag("Player")) {
            GameManager.Instance.score += this.score;
            if(collision.gameObject.transform.position.y > ball.position.y) {
                collision.gameObject.transform.position = ball.position + new Vector3(-0.25f, 0, 0);
            }
            animator.SetTrigger("isFinish");
        }    
    }

    public void FlagStop() {
        animator.enabled = false;
    }
}
