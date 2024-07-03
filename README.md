# 슈퍼마리오1 모작

  ## 개요 
  - 개인 프로젝트로 슈퍼마리오1 의 1-1 스테이지를 구현한 것입니다.
  - 언어를 공부한 것을 바탕으로 다양하게 로직을 구성해보았습니다.
  - delegate, Action, interface 등을 사용하여 구현하였습니다 .



  ## 개발엔진 
  - Unity
  - Visual Studio 2022

  ## 기능 구현 

  ### Manager
  - KeyManager
    - InputManager에 관련된 키값들을 한번에 모아서 관리하기 위한 Manager Script 입니다.
    - 딕셔너리로 각 타입에 맞는 값는 키값을 묶어 키를 관리하였습니다.
    - 접근은 InputKeyManager를 (keyManager)객체로 받아와 keyManager.GetKeyCode(KeyCodeTypes.LeftMove)
    ```C#
    public static InputKeyManager instance;

    // 열거형 변수 선언 
    public enum KeyCodeTypes {
        LeftMove,
        RightMove,
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
    }
    public KeyCode GetKeyCode( KeyCodeTypes action ) {
        // 키값 반환 
        return keyMappings[action];
    }
    public void SetKeyCode( KeyCodeTypes action, KeyCode keyCode ) {
        // 키값 설정 
        keyMappings[action] = keyCode;
    }
    ```

  - GameManager
    - 

  - Cameramanager

  ---

  ### 이동 및 점프 
  <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%EC%9D%B4%EB%8F%99%EC%A0%90%ED%94%84.gif" width="200" height="200">
  </p>
  
  - 기본적인 이동과 점프 모션입니다. 

  - 이동 로직은 delegate를 사용하여 구현하였습니다.
  ```C#
  public class Player : MonoBehaviour {
    public delegate void GameMoveEvent(bool value);
    public static event GameMoveEvent OnMarioMove;

    void OnEnable() {
       OnMarioMove += MarioMove;
    }
    void OnDisable() {
       OnMarioMove -= MarioMove;
    }
    void FixedUpdate() {
      if (!isDead) {
          bool isRunning = Input.GetKey(keyManager.GetKeyCode(KeyCodeTypes.Run));
          OnMarioMove?.Invoke(isRunning);
      }
    }
  ```

  - 슈퍼마리오1 의 특징으로 약간의 미끄러짐 효과를 구현하기 위해서 이동을 AddForce로 구현하였습니다. 
  - 또한 X 키를 눌러 달리기를 할때 일반적인 AddForce로 주면 속도가 초기화가 되기 때문에 velocity로 가속도를 유지해주었습니다. 
  ```C#
   void MarioMove(bool isRun) {
       float animSpeed = isRun ? 1.5f : 0.5f;
       float stateSpeed = isRun ? runSpeed : speed;
       float maxSpeed = isRun ? maxRunSpeed : maxWalkSpeed;
  
       isMove = true;
       animator.SetBool("isWalk", true);
       animator.speed = animSpeed;
  
       if (Input.GetKey(keyManager.GetKeyCode(KeyCodeTypes.LeftMove))) {
           // 방향 전환 시 애니메이션 동작 
           if (isTureRight) {
               isTureRight = false;
               animator.SetBool("isSkid", true);
               Invoke("returnAnimeTurn", 0.3f);
           }
           // 스프라이트 방향 전환, 기본 이동 
           spriteRenderer.flipX = true;
           rigid.AddForce(Vector3.left * stateSpeed * Time.deltaTime, ForceMode2D.Impulse);
       }
       else if (Input.GetKey(keyManager.GetKeyCode(KeyCodeTypes.RightMove))) {
           if (!isTureRight) {
               isTureRight = true;
               animator.SetBool("isSkid", true);
               spriteRenderer.flipX = true;
               Invoke("returnAnimeTurn", 0.3f);
           }
           spriteRenderer.flipX = false;
           rigid.AddForce(Vector3.right * stateSpeed * Time.deltaTime, ForceMode2D.Impulse);
       }
       else {
           isMove = false;
           animator.SetBool("isWalk", false);
       }
  
       // 최대 속도 제한
       Vector2 clampedVelocity = rigid.velocity;
       if (clampedVelocity.x > maxSpeed) {
           clampedVelocity.x = maxSpeed;
       }
       else if (clampedVelocity.x < -maxSpeed) {
           clampedVelocity.x = -maxSpeed;
       }
       rigid.velocity = clampedVelocity;
   }
```
  ---

  ### 공격 
  - 파이어볼 
  <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%ED%8C%8C%EC%9D%B4%EC%96%B4%EB%B3%BC.gif" width="200" height="200">
  </p>
  
  - 꽃 아이템을 먹었을 때 불을 발사할 수 있는 상태로 변합니다.
  - hp 라는 변수를 사용하여 마리오의 체력을 구현하면서 변수를 활용하여 마리오의 스프라이트의 변화를 주었습니다.
  - 마리오는 스프라이트가 전환되는 방식이 아닌 애니메이션 방식으로 구현하였습니다. 
  ```C#
      if (collision.gameObject.CompareTag("FlowerItem")) {
            GameManager.Instance.score += 1000;
            if (hp == 2) {
                collision.gameObject.SetActive(false);
                return;
            }

            Time.timeScale = 0; 
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            collision.gameObject.SetActive(false);
            if (hp == 0) {
                OnMarioMove -= MarioMove;
                isJumping = true;
                animator.SetTrigger("isMushroom");
                MarioChange(1, 1f, 0.5f, 0.25f, 0.75f);
            }
            else if (hp == 1) {
                OnMarioMove -= MarioMove;
                isJumping = true;
                animator.SetTrigger("isFlower");
                MarioChange(2, 1f, 0.5f, 0.25f, 0.75f);
            }
            hp += 1;
        }
  ```
  
  - 거북이 등껍질
  <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%EA%BB%8D%EC%A7%88.gif" width="200" height="200">
  </p>

  - 밟으면 날아가는 거북이 등껍질 아이템 입니다.
  - 플레이어와 x 좌표를 비교하여 날아가는 방향을 결정합니다.
  - 날아갈때는 bool 변수로 날아가는 상태인것을 체크하여 Enemy와 Player에게 부딪쳤을 때 데미지를 줍니다.
  - 좌우에 raycast를 활성화 하여 pipe 또는 ground에 닿았을 때 방향을 전환합니다. 

  ---
  
  ### 몬스터 

  ---

  ### 아이템 

  ---

  ### 여러 이벤트 

  - 지하와 깃발
  - <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%EC%A7%80%ED%95%98.gif" width="200" height="200">
  </p>

  ---
  
  ### UI

  ---

  ### 만들면서 배우게 된 것 
