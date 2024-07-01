using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject effectPre;

    bool isHit;
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D( Collider2D collision ) {
        if (transform.position.y > collision.transform.position.y && !isHit) {
            isHit = true;
            collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity = Vector3.zero;

            if (GameManager.Instance.player.GetComponent<Player>().hp >= 1) {
                GameObject effect = GameManager.Instance.pooling.GetObject(11);
                effect.transform.position = transform.position;
                effect.transform.rotation = Quaternion.identity;    
                GameManager.Instance.score += 50;
                gameObject.SetActive(false);
            }
            else {
                animator.SetTrigger("isBounce");
                StartCoroutine(IsHitReturn());
            }
        }
    }

    IEnumerator IsHitReturn() {
        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }

    public void AnimaStop() {
        animator.enabled = false;
    }
}
