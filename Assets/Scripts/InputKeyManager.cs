using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyManager : MonoBehaviour
{
    public static InputKeyManager instance;

    // 열거형 변수 선언 
    public enum KeyCodeTypes {
        LeftMove,
        RightMove,
        DownMove,
        Down,
        Jump,
        Attack,
        Run
    }

    // 딕셔너리로 키 관리 
    private Dictionary<KeyCodeTypes, KeyCode> keyMappings;

    void Awake() {
        instance = this;
        // 딕셔너리 초기화 
        keyMappings = new Dictionary<KeyCodeTypes, KeyCode>();

        // 각 디셔너리 키에 맞는 키보드 값을 추가 
        keyMappings[KeyCodeTypes.LeftMove] = KeyCode.LeftArrow;
        keyMappings[KeyCodeTypes.RightMove] = KeyCode.RightArrow;
        keyMappings[KeyCodeTypes.DownMove] = KeyCode.DownArrow;
        keyMappings[KeyCodeTypes.Jump] = KeyCode.Z;
        keyMappings[KeyCodeTypes.Attack] = KeyCode.C;
        keyMappings[KeyCodeTypes.Run] = KeyCode.X;
    }

    public KeyCode GetKeyCode( KeyCodeTypes action ) {
        // 키값 반환 
        return keyMappings[action];
    }

    public void SetKeyCode( KeyCodeTypes action, KeyCode keyCode ) {
        // 키값 설정 
        keyMappings[action] = keyCode;
    }
}
