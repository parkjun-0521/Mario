using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance {
        get {
            if (null == instance) {
                return null;
            }
            return instance;
        }
    }

    public Pooling pooling;
    public GameObject player;
    public Transform targetPosition;
    public Transform underSpawn;
    public Transform upSpawn;

    public int score;
    public int coinScore;

    void Awake() {
        if (null == instance) {     
            instance = this;
           /* player = GameObject.FindGameObjectWithTag("Player");
            targetPosition = GameObject.FindGameObjectWithTag("End").GetComponent<Transform>();*/
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
    }
    private void Start() {
        Screen.SetResolution(256, 240, FullScreenMode.Windowed);
    }
}
