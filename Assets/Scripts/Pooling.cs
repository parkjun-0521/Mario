using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    public GameObject[] prefabs;
    List<GameObject>[] pools;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        pools = new List<GameObject>[prefabs.Length];
        for (int index = 0; index < prefabs.Length; index++) {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject GetObject( int index ) {
        GameObject select = null;
        foreach (GameObject objects in pools[index]) {
            if (!objects.activeSelf) {
                select = objects;
                objects.SetActive(true);
                break;
            }
        }
        if (!select) {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }
        return select;
    }
}
