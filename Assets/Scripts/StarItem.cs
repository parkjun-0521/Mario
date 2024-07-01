using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour
{
    Rigidbody2D rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rigid.AddForce(Vector3.right * 3f, ForceMode2D.Impulse);
    }
}
