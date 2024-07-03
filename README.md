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


  ### 오브젝트 풀링 

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
  - [등껍질 구현 Code](https://github.com/parkjun-0521/Mario/blob/master/Assets/Scripts/Shell.cs)
  ---
  
  ### 몬스터 
  - [Enemy interface 및 class](https://github.com/parkjun-0521/Mario/blob/master/Assets/Scripts/Enemy.cs)

  - 굼바
    - 단순한 좌우 이동 로직과 충돌로직이 있습니다.
    - 이동 방향 중 오브젝트와 충돌했을 때 방향을 전환하기 위해 양옆으로 Ray를 쏴 오브젝트를 구별하여 방향 전환을 하였습니다.
    - 부모의 함수를 override 하여 필요한 부분들만 구현하였습니다. 

  - 거북이
    - 굼바와 거의 모든 기능이 일치합니다.
    - 다른점은 거북이에서는 부모의 public virtual void ShellDrop(GameObject shell ) { return; } 함수를 구현하였습니다. 

  ---

  ### 아이템 

  - 버섯, 꽃, 코인, 별
    - 모든 아이템의 충돌 처리 로직은 Player에서 작성하였습니다.
   
    - 아이템을 Player에서 처리하면서 프로젝트의 오브젝트간의 결합도를 낮추었습니다.
      
    - 버섯 : hp가 1증가하면서 슈퍼마리오로 스프라이트를 전환합니다. ( 이 또한 애니메이션으로 구현하였습니다. )
    - 꽃 : hp가 1 증가하면서 플라워마리오로 스프라이트를 전환합니다. ( 이 또한 애니메이션으로 구현하였습니다. )
    - 코인 : 블록 오브젝트가 캐릭터와 충돌 시 등장하면서 애니메이션 동작 후 애니메이션 이벤트를 활용하여 애니메이션이 끝난 시점에서 오브젝트를 비활성화 합니다.
    - 별 : 캐릭터에게 무적효과를 줍니다. 

  ---

  ### 여러 이벤트 

  - 지하와 깃발
  - <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%EC%A7%80%ED%95%98.gif" width="200" height="200">
  </p>

  - 깃발에 닿았을 때와 깃발 밑의 바닥에 닿았을 때를 구분하여 동작을 하게 하였다.
  - 깃발에 닿을 시
  - gravityScale를 조정하여 자연스럽게 아래로 떨어지게 구현하였다.
```C#
    if (collision.gameObject.CompareTag("FinishLine")){
            UIManager uIManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
            uIManager.StopTime();
            isFinish = true;
            OnMarioMove -= MarioMove;
            rigid.velocity = Vector3.zero;
            rigid.gravityScale = 0.5f;
            animator.SetBool("isFinish", true);
        }
```



- 바닥에 닿았을 때
- 성 까지 들어가는 부분은 코루틴을 사용하여 이동시키면서 애니메이션 같은 효과를 주었습니다.
- 애니메이션은 자원이 많이들어가기 때문에 이러한 단순 거리 이동은 MoveTowards로 구현하는거 더 좋다고 생각했습니다. 
 ```C#
   if (collision.gameObject.CompareTag("FinishBlock")) {
            rigid.gravityScale = 3f;
            GameObject collider2D = GameObject.FindGameObjectWithTag("FinishLine");
            collider2D.GetComponent<BoxCollider2D>().enabled = false;
            animator.SetBool("isFinish", false);
            StartCoroutine(End());
        }
   IEnumerator End() {
        rigid.MovePosition(rigid.position + new Vector2(0.75f, 0));
        spriteRenderer.flipX = true;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.flipX = false;
        // 이동을 보장하기 위해 while 루프 사용
        animator.SetBool("isJump", false);
        while (Vector3.Distance(transform.position, GameManager.Instance.targetPosition.position) > 0.01f) {
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.targetPosition.position, 2f * Time.unscaledDeltaTime);
            animator.SetBool("isWalk", true);
            yield return null; // 다음 프레임까지 기다림
        }
    }
```

  ---
  
  ### UI

  - 타이머
    - 시간은 400초로 시작해서 1씩 감소한다.
    - 단, 아이템을 먹거나 깃발에 닿을 시 시간이 일시적으로 멈춘다. 보통 TimeScale을 0으로 만들어 시간을 정지한다.
    - 이때 Player는 애니메이션을 동작해야하기 때문에 
    - animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    - updateMode 를 사용하여 TimeScale에 영향을 받지 않게 하여 아이템을 먹었을 때도 애니메이션을 동작시키도록 구현 
    
  ---
