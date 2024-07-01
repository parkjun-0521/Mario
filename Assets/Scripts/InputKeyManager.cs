using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyManager : MonoBehaviour
{
    public static InputKeyManager instance;

    // ������ ���� ���� 
    public enum KeyCodeTypes {
        LeftMove,
        RightMove,
        DownMove,
        Down,
        Jump,
        Attack,
        Run
    }

    // ��ųʸ��� Ű ���� 
    private Dictionary<KeyCodeTypes, KeyCode> keyMappings;

    void Awake() {
        instance = this;
        // ��ųʸ� �ʱ�ȭ 
        keyMappings = new Dictionary<KeyCodeTypes, KeyCode>();

        // �� ��ųʸ� Ű�� �´� Ű���� ���� �߰� 
        keyMappings[KeyCodeTypes.LeftMove] = KeyCode.LeftArrow;
        keyMappings[KeyCodeTypes.RightMove] = KeyCode.RightArrow;
        keyMappings[KeyCodeTypes.DownMove] = KeyCode.DownArrow;
        keyMappings[KeyCodeTypes.Jump] = KeyCode.Z;
        keyMappings[KeyCodeTypes.Attack] = KeyCode.C;
        keyMappings[KeyCodeTypes.Run] = KeyCode.X;
    }

    public KeyCode GetKeyCode( KeyCodeTypes action ) {
        // Ű�� ��ȯ 
        return keyMappings[action];
    }

    public void SetKeyCode( KeyCodeTypes action, KeyCode keyCode ) {
        // Ű�� ���� 
        keyMappings[action] = keyCode;
    }
}
