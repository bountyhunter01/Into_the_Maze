using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    Rigidbody rb;

    [Header("Rotate")]
    public float mouseSpeed;
    float yRotation;
    float xRotation;
    public Camera cam;

    [Header("Move")]
    public float moveSpeed;
    public float dashSpeed;
    float h;
    float v;
    private bool isAction = true;

    [Header("Jump")]
    public float jumpForce;

    [Header("Ground Check")]
    public bool grounded;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float dashStaminaCost = 20f;
    public float staminaRegenRate = 10f;
    public Slider staminaSlider;
    private bool isDashing = false;
    private bool isDashButtonDown = false;

    [Header("Canvas")]
    public GameObject gameOverObj;
    public GameObject pauseMenu; // 새로 추가: 메뉴 UI

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                     // 마우스 커서를 보이지 않도록 설정

        rb = GetComponent<Rigidbody>();             // Rigidbody 컴포넌트 가져오기
        rb.freezeRotation = true;                   // Rigidbody의 회전을 고정하여 물리 연산에 영향을 주지 않도록 설정

        cam = Camera.main;                          // 메인 카메라를 할당
        currentStamina = maxStamina;                // 스테미너 초기화

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina / 10; // 스테미너 슬라이더 최대값 설정
            staminaSlider.value = staminaSlider.maxValue; // 초기 스테미너 값 설정
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false); // 초기에는 메뉴를 비활성화
        }
    }

    void Update()
    {
        if (!isAction) return;  // 플레이어 움직임 차단

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        Rotate();
        Move();
        RegenerateStamina();                        // 스테미너 재생

        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetButtonDown("Dash"))
        {
            isDashButtonDown = true;
        }
        if (Input.GetButtonUp("Dash"))
        {
            isDashButtonDown = false;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 힘을 가해 점프
        grounded = false; // 점프한 후에는 공중에 떠 있는 상태로 설정
    }

    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // 수평 이동 입력 값
        v = Input.GetAxisRaw("Vertical");   // 수직 이동 입력 값

        // 입력에 따라 이동 방향 벡터 계산
        Vector3 moveVec = transform.forward * v + transform.right * h;

        if (isDashButtonDown && currentStamina >= dashStaminaCost && !isDashing)
        {
            StartCoroutine(Dash(moveVec));
        }
        else
        {
            // 이동 벡터를 정규화하여 이동 속도와 시간 간격을 곱한 후 물체의 질량을 무시하고 직접 속도를 변경합니다
            rb.AddForce(moveVec.normalized * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    IEnumerator Dash(Vector3 moveVec)
    {
        isDashing = true;
        float startTime = Time.time;
        currentStamina -= dashStaminaCost;
        UpdateStaminaSlider();

        while (Time.time < startTime + 0.2f) // 대시 지속 시간
        {
            rb.AddForce(moveVec.normalized * dashSpeed * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }

        isDashing = false;
    }

    void RegenerateStamina()
    {
        if (!isDashing && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateStaminaSlider();
        }
    }

    void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        yRotation += mouseX;    // 마우스 X축 입력에 따라 수평 회전 값을 조정
        xRotation -= mouseY;    // 마우스 Y축 입력에 따라 수직 회전 값을 조정

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 수직 회전 값을 -90도에서 90도 사이로 제한

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); // 카메라의 회전을 조절
        transform.rotation = Quaternion.Euler(0, yRotation, 0);             // 플레이어 캐릭터의 회전을 조절
    }

    void UpdateStaminaSlider()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / 10; // 스테미너 값을 슬라이더 값으로 업데이트
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
        if (collision.gameObject.tag == "Monster")
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        // 게임오버 텍스트 및 재시작 버튼 활성화, 게임 씬 리로드
        gameOverObj.SetActive(true);
        isAction = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;  // 마우스 커서 해제
        cam.transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.isKinematic = true;  // Rigidbody를 비활성화하여 물리 연산 중지
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            // 메뉴 비활성화
            pauseMenu.SetActive(false);
            Time.timeScale = 1f; // 게임 재개
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isAction = true;
        }
        else
        {
            // 메뉴 활성화
            pauseMenu.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isAction = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        TogglePauseMenu();
    }
}
