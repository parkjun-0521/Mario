using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    public GameObject[] DropItem;
    public int id = 0;

    bool isHit;
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start() {
        if(DropItem.Length > 1) {
            id = 1;
        }
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (transform.position.y > collision.transform.position.y && !isHit) {
            isHit = true;
            collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity = Vector3.zero;

            if (id == 0) {
                Instantiate(DropItem[0], transform.position + Vector3.up, transform.rotation);
            }
            else if(id == 1) {
                if(GameManager.Instance.player.GetComponent<Player>().hp == 1 ||
                   GameManager.Instance.player.GetComponent<Player>().hp == 2) {
                    GameObject flower = GameManager.Instance.pooling.GetObject(3);
                    flower.transform.position = transform.position + Vector3.up;
                    flower.transform.rotation = Quaternion.identity;
                }
                else {
                    GameObject mushroom = GameManager.Instance.pooling.GetObject(2);
                    mushroom.transform.position = transform.position + Vector3.up;
                    mushroom.transform.rotation = Quaternion.identity;
                }
            }
            animator.SetTrigger("isBounce");
        }
    }

    public void AnimaStop() {
        animator.enabled = false;
    }
}
