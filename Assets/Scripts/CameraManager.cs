using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Vector3 cameraPosition;

    [SerializeField]
    Vector2 center;
    [SerializeField]
    Vector2 mapSize;

    [SerializeField]
    float cameraMoveSpeed;
    float height;
    float width;

    // 카메라의 최소 X 위치를 추적합니다.
    private float minX;
    private Vector3 initialPosition;

    void Start() {
        playerTransform = GameManager.Instance.player.GetComponent<Transform>();

        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;

        initialPosition = transform.position;

        // 초기 카메라 X 위치를 설정합니다.
        minX = transform.position.x;
    }

    void FixedUpdate() {
        LimitCameraArea();
    }

    void LimitCameraArea() {
        Vector3 targetPosition = playerTransform.position + cameraPosition;
        targetPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraMoveSpeed);

        // 카메라가 이동할 수 있는 최대 X 위치를 계산합니다.
        float lx = mapSize.x - width;
        float clampX = Mathf.Clamp(targetPosition.x, -lx + center.x, lx + center.x);

        // 카메라가 왼쪽으로 이동하지 않도록 minX를 업데이트합니다.
        if (clampX > minX) {
            minX = clampX;
        }

        if(!GameManager.Instance.player.GetComponent<Player>().isUnder)
            transform.position = new Vector3(minX, 6, -10f);
        else
            transform.position = new Vector3(minX, -13, -10f);
    }

    // 플레이어 사망 시 호출할 메서드
    public void ResetCameraPosition() {
        transform.position = initialPosition;
        minX = initialPosition.x; // minX도 초기 위치로 리셋
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, mapSize * 2);
    }

    public void UnderGroundPosition()
    {
        center = new Vector2(-39.8f, -13f);
        mapSize = new Vector2(8.2f, 7);
    }
    public void UpGroundPosition()
    {
        center = new Vector2(0, 6);
        mapSize = new Vector2(102, 7);
    }
}
