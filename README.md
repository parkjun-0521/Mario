# 슈퍼마리오1 모작

  ## 개요 
  - 개인 프로젝트로 슈퍼마리오1 의 1-1 스테이지를 구현한 것입니다.
  - 언어를 공부한 것을 바탕으로 다양하게 로직을 구성해보았습니다.
  - delegate, Action, interface 등을 사용하여 구현하였습니다 .

  ## 개발엔진 
  - Unity
  - Visual Studio 2022

  ## 기능 구현 

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
  

  ### 공격 
  - 파이어볼 
  <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%ED%8C%8C%EC%9D%B4%EC%96%B4%EB%B3%BC.gif" width="200" height="200">
  </p>
  
  - 거북이 등껍질
  <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%EA%BB%8D%EC%A7%88.gif" width="200" height="200">
  </p>
  
  ### 몬스터 

  ### 아이템 

  ### 여러 이벤트 

  - 지하와 깃발
  - <p align="left" >
    <img src = "https://github.com/parkjun-0521/Mario/blob/master/Image/%EC%A7%80%ED%95%98.gif" width="200" height="200">
  </p>
  
  ### UI

  ### 만들면서 배우게 된 것 
