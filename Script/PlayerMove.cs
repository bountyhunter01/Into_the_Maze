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
    public GameObject pauseMenu; // ���� �߰�: �޴� UI

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // ���콺 Ŀ���� ȭ�� �ȿ��� ����
        Cursor.visible = false;                     // ���콺 Ŀ���� ������ �ʵ��� ����

        rb = GetComponent<Rigidbody>();             // Rigidbody ������Ʈ ��������
        rb.freezeRotation = true;                   // Rigidbody�� ȸ���� �����Ͽ� ���� ���꿡 ������ ���� �ʵ��� ����

        cam = Camera.main;                          // ���� ī�޶� �Ҵ�
        currentStamina = maxStamina;                // ���׹̳� �ʱ�ȭ

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina / 10; // ���׹̳� �����̴� �ִ밪 ����
            staminaSlider.value = staminaSlider.maxValue; // �ʱ� ���׹̳� �� ����
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false); // �ʱ⿡�� �޴��� ��Ȱ��ȭ
        }
    }

    void Update()
    {
        if (!isAction) return;  // �÷��̾� ������ ����

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        Rotate();
        Move();
        RegenerateStamina();                        // ���׹̳� ���

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
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // ���� ���� ����
        grounded = false; // ������ �Ŀ��� ���߿� �� �ִ� ���·� ����
    }

    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // ���� �̵� �Է� ��
        v = Input.GetAxisRaw("Vertical");   // ���� �̵� �Է� ��

        // �Է¿� ���� �̵� ���� ���� ���
        Vector3 moveVec = transform.forward * v + transform.right * h;

        if (isDashButtonDown && currentStamina >= dashStaminaCost && !isDashing)
        {
            StartCoroutine(Dash(moveVec));
        }
        else
        {
            // �̵� ���͸� ����ȭ�Ͽ� �̵� �ӵ��� �ð� ������ ���� �� ��ü�� ������ �����ϰ� ���� �ӵ��� �����մϴ�
            rb.AddForce(moveVec.normalized * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    IEnumerator Dash(Vector3 moveVec)
    {
        isDashing = true;
        float startTime = Time.time;
        currentStamina -= dashStaminaCost;
        UpdateStaminaSlider();

        while (Time.time < startTime + 0.2f) // ��� ���� �ð�
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

        yRotation += mouseX;    // ���콺 X�� �Է¿� ���� ���� ȸ�� ���� ����
        xRotation -= mouseY;    // ���콺 Y�� �Է¿� ���� ���� ȸ�� ���� ����

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // ���� ȸ�� ���� -90������ 90�� ���̷� ����

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); // ī�޶��� ȸ���� ����
        transform.rotation = Quaternion.Euler(0, yRotation, 0);             // �÷��̾� ĳ������ ȸ���� ����
    }

    void UpdateStaminaSlider()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / 10; // ���׹̳� ���� �����̴� ������ ������Ʈ
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
        // ���ӿ��� �ؽ�Ʈ �� ����� ��ư Ȱ��ȭ, ���� �� ���ε�
        gameOverObj.SetActive(true);
        isAction = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;  // ���콺 Ŀ�� ����
        cam.transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.isKinematic = true;  // Rigidbody�� ��Ȱ��ȭ�Ͽ� ���� ���� ����
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
            // �޴� ��Ȱ��ȭ
            pauseMenu.SetActive(false);
            Time.timeScale = 1f; // ���� �簳
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isAction = true;
        }
        else
        {
            // �޴� Ȱ��ȭ
            pauseMenu.SetActive(true);
            Time.timeScale = 0f; // ���� �Ͻ�����
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
