using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static InputKeyManager;

public class Player : MonoBehaviour {
    public delegate void GameMoveEvent(bool value);
    public static event GameMoveEvent OnMarioMove;

    bool isMove;
    public float speed;
    public float maxWalkSpeed;
    bool isRun;
    public float runSpeed;
    public float maxRunSpeed;
    bool isTureRight = true;

    public float maxJumpPower = 10f;
    public float minJumpPower = 5f;
    public bool isJumping;
    public bool isLongJump;
    public float jumpDuration = 0.5f;
    private float jumpStartTime;

    public float fireDelay = 0.1f; // 0.5초 딜레이
    bool canFire = true;

    public int health;  // 전체 목숨 
    public int hp;      // 크기 증가 hp ( 아직 사용 안함 ) 

    public bool isStar;
    public bool isFinish;

    public bool isUnder;

    public bool checkPoint;
    public bool isDead;

    public BoxCollider2D headPos;
    public BoxCollider2D footPos;
    public AnimatorOverrideController[] animatorOverrideControllers;

    InputKeyManager keyManager;

    public SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    CapsuleCollider2D capsuleCollider;
    Animator animator;

    public GameObject fireBall;
    
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void OnEnable() {
        OnMarioMove += MarioMove;
    }
    void OnDisable() {
        OnMarioMove -= MarioMove;
    }

    void Start() {
        keyManager = InputKeyManager.instance.GetComponent<InputKeyManager>();
        health = 3;
        hp = 0;
        isStar = false;
    }

    void Update() {
        if (!isDead) {
            MarioJump();
        }

        if(hp == 2 && canFire) {
            MarioFier();
        }
    }

    void FixedUpdate() {
        if (!isDead) {
            bool isRunning = Input.GetKey(keyManager.GetKeyCode(KeyCodeTypes.Run));
            OnMarioMove?.Invoke(isRunning);
        }
    }

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
    void returnAnimeTurn() {
        // 턴 애니메이션 끄기 
        animator.SetBool("isSkid", false);
    }
    // 점프 로직
    void MarioJump() {
        if (isFinish) {
            return;
        }

        KeyCode jumpKey = keyManager.GetKeyCode(KeyCodeTypes.Jump);

        // 짧은 점프 
        if (Input.GetKeyDown(jumpKey) && !isJumping) {
            footPos.gameObject.SetActive(false);
            jumpStartTime = Time.time;
            isJumping = true;
            rigid.AddForce(Vector2.up * minJumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJump", true);
            isLongJump = true;
        }
        // 긴 점프 
        if (Input.GetKey(jumpKey) && isJumping && isLongJump) {
            float holdTime = Time.time - jumpStartTime;
            if (holdTime < jumpDuration) {
                float jumpForce = Mathf.Lerp(minJumpPower, maxJumpPower, holdTime / jumpDuration);
                rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
            }
        }
        // 점프 연속 실행 막기 
        if (Input.GetKeyUp(jumpKey)) {
            isLongJump = false;
        }

        if(rigid.velocity.y < 0) {
            footPos.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter2D( Collider2D collision ) {
        // 지면, 파이프, 블록에 FootPos가 충돌했을 때 
        if (collision.CompareTag("Ground") || collision.CompareTag("Pipe") || collision.CompareTag("Block")) {
            animator.SetBool("isJump", false);
            // 착지했을 때 이동 속도를 최고속도로 변경 
            if (!spriteRenderer.flipX) {
                // 왼쪽 
                if (isMove)
                    rigid.velocity = (!isRun) ? new Vector2(maxWalkSpeed, 0) : new Vector2(maxRunSpeed, 0);
            }
            else {
                // 오른쪽
                if (isMove)
                    rigid.velocity = (!isRun) ? new Vector2(-maxWalkSpeed, 0) : new Vector2(-maxRunSpeed, 0);
            }
            // 점프 상태 복귀 
            isJumping = false;
        }
        else if (collision.gameObject.CompareTag("End")) {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("UnderGround") && Input.GetKey(keyManager.GetKeyCode(KeyCodeTypes.DownMove))) {
            isUnder = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>().UnderGroundPosition();
            transform.position = GameManager.Instance.underSpawn.position;
        }
        else if(collision.CompareTag("UpGround") && Input.GetKey(keyManager.GetKeyCode(KeyCodeTypes.RightMove))) {
            isUnder = false;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>().UpGroundPosition();
            transform.position = GameManager.Instance.upSpawn.position;
        }
        else if (collision.gameObject.CompareTag("Coin")) {
            GameManager.Instance.score += 200;
            collision.gameObject.SetActive(false);
        }
    }


    private void OnCollisionEnter2D( Collision2D collision ) {
        if (collision.gameObject.CompareTag("Enemy") && !isStar) {
            if (transform.position.y > collision.transform.position.y) {
                MarioAttack();
            }
            else {
                // 맞았을 때 넉백 
                if (transform.position.x - collision.transform.position.x < 0) 
                    rigid.AddForce(new Vector2(0, 1) * 5f, ForceMode2D.Impulse);    // 왼쪽 
                else 
                    rigid.AddForce(new Vector2(0, 1) * 5f, ForceMode2D.Impulse);     // 오른쪽 

                // 데미지 
                hp -= 1;
                if (hp < 0) {
                    MarioDead();
                }
                else if (hp == 0) {
                    MarioChange(0, 0.5f, 0.25f, 0.25f, 0.25f);    // 작은 마리오 
                }
                else if (hp == 1) {
                    MarioChange(1, 1f, 0.5f, 0.25f, 0.75f);     // 큰 마리오
                }
            }
        }
        else if (collision.gameObject.CompareTag("MushroomItem")) {
            if (hp != 0) {
                return;
            }

            Time.timeScale = 0; 
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            OnMarioMove -= MarioMove;
            hp += 1;
            isJumping = true;
            animator.SetTrigger("isMushroom");
            MarioChange(1, 1f, 0.5f, 0.25f, 0.75f);
        }
        else if (collision.gameObject.CompareTag("FlowerItem")) {
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
        else if (collision.gameObject.CompareTag("HealthUpItem")) {
            health += 1;
        }
        else if (collision.gameObject.CompareTag("StarItem")) {
            GameManager.Instance.score += 1000;
            collision.gameObject.SetActive(false);
            isStar = true;
            StartCoroutine(StarTimeEnd());
            Debug.Log("별 먹음 애니메이션 만들어주기 ");
        }
        else if (collision.gameObject.CompareTag("FinishLine")){
            UIManager uIManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
            uIManager.StopTime();
            isFinish = true;
            OnMarioMove -= MarioMove;
            rigid.velocity = Vector3.zero;
            rigid.gravityScale = 0.5f;
            animator.SetBool("isFinish", true);
        }
        else if (collision.gameObject.CompareTag("FinishBlock")) {
            rigid.gravityScale = 3f;
            GameObject collider2D = GameObject.FindGameObjectWithTag("FinishLine");
            collider2D.GetComponent<BoxCollider2D>().enabled = false;
            animator.SetBool("isFinish", false);
            StartCoroutine(End());
        }
        else if (collision.gameObject.CompareTag("DeadZoon")) {
            UIManager uIManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
            uIManager.StopTime();
            health -= 1;
            rigid.velocity = Vector2.zero;
            isDead = true;
            capsuleCollider.enabled = false;
            animator.SetBool("isDead", true);
            rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
            StartCoroutine(RespawnPoint());
        }
    }

    public void MarioDead() {
        UIManager uIManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        uIManager.StopTime();
        health -= 1;
        rigid.velocity = Vector2.zero;
        isDead = true;
        capsuleCollider.enabled = false;
        animator.SetBool("isDead", true);
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        StartCoroutine(RespawnPoint());
    }

    IEnumerator RespawnPoint() {
        yield return new WaitForSeconds(5f);
        UIManager uIManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        uIManager.StartTime();
        // 카메라 위치 초기화 
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>().ResetCameraPosition();
        animator.SetBool("isDead", false);
        capsuleCollider.enabled = true;
        isDead = false;
        // 리스폰 
        spriteRenderer.flipX = false;
        transform.position = (!checkPoint) ? new Vector2(-99, 1) : new Vector2(-21, 1);
    }

    IEnumerator StarTimeEnd() {
        yield return new WaitForSeconds(5f);
        Debug.Log("애니메이션 원상복귀");
        isStar = false;
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

    void IsItemState() {
        isJumping = false;
        OnMarioMove += MarioMove;
        Time.timeScale = 1;
    }

    // Animator, 콜라이더 크기, 콜라이더 위치, 아래 충돌 범위, 위 충돌 범위 
    public void MarioChange(int index, float size, float offsetPos, float footPosion, float headPosion) {
        animator.runtimeAnimatorController = animatorOverrideControllers[index];
        capsuleCollider.size = new Vector2(0.40625f, size);
        capsuleCollider.offset = new Vector2(0, offsetPos);
        footPos.transform.localPosition = new Vector3(0f, footPosion, 0f);
        headPos.transform.localPosition = new Vector3(0f, headPosion, 0f);
    }

    // 몬스터 공격 
    void MarioAttack() {
        rigid.velocity = Vector2.zero;
        if (!spriteRenderer.flipX) {
            rigid.AddForce(new Vector2(0, 6), ForceMode2D.Impulse);
        }
        else {
            rigid.AddForce(new Vector2(0, 6), ForceMode2D.Impulse);
        }
    }

    void MarioFier() {
        if (Input.GetKeyDown(keyManager.GetKeyCode(KeyCodeTypes.Run))) {
            StartCoroutine(FireDelay());
        }
    }

    IEnumerator FireDelay() {
        canFire = false;
        Debug.Log("파이어볼 날리기");
        animator.SetTrigger("isFire");
        GameObject fireBall = GameManager.Instance.pooling.GetObject(0);
        fireBall.transform.position = transform.position + new Vector3(0, 1, 0);
        fireBall.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(fireDelay);
        canFire = true;
    }
}