using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System kütüphanesi
using UnityEngine.SceneManagement;
using TMPro;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10.0f;
    public float jumpForce = 500f;

    [Header("Ground Detection")]
    public Transform groundCheckPoint;
    public LayerMask groundLayer;

    [Header("Wall Jump Settings")]
    public float wallJumpForceX = 700f;
    public float wallJumpForceY = 500f;
    public Transform wallCheckPoint;
    public LayerMask wallLayer;
    public float wallJumpCooldown = 0.2f;

    [Header("Game Elements")]
    public LayerMask hunterLayer;
    public LayerMask goalLayer;
    public GameObject loseTextUI;
    public GameObject winTextUI;
    public float fallLimitY = -10f;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    // State Variables
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDirection;
    private float wallJumpTimer;

    // Input System
    private PlayerControls inputActions;
    private Vector2 moveInput;

    private void Awake()
    {
        // Generated Class'ı başlatıyoruz
        inputActions = new PlayerControls();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallJumpTimer = 0f;

        // Null Check'ler
        if (groundCheckPoint == null) Debug.LogError("Ground Check Point atanmamış!");
        if (wallCheckPoint == null) Debug.LogError("Wall Check Point atanmamış!");

        if (loseTextUI != null) loseTextUI.SetActive(false);
        if (winTextUI != null) winTextUI.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        // Eventlere abone oluyoruz (Tuşa basıldığı an çalışır)
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Restart.performed += OnRestartPerformed;
    }

    private void OnDisable()
    {
        // Event aboneliklerini kaldırıyoruz
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Restart.performed -= OnRestartPerformed;

        inputActions.Player.Disable();
    }

    void Update()
    {
        // Düşme kontrolü (Update içinde kalabilir)
        if (transform.position.y < fallLimitY)
        {
            RestartGameInstant();
        }
    }

    void FixedUpdate()
    {
        // Fiziksel kontrolleri FixedUpdate'te yapmak daha sağlıklıdır
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundLayer);

        // Duvar kontrolü
        if (wallCheckPoint != null)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheckPoint.position, 0.2f, wallLayer);
            if (isTouchingWall)
            {
                // Duvarın ne tarafta olduğunu bul
                wallDirection = (wallCheckPoint.position.x > transform.position.x) ? 1 : -1;
            }
        }

        // Timer mantığı
        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }

        // Hareket Girdisini Oku (ReadValue)
        // inputActions dosyasında Move action'ını Vector2 ayarladığını varsayıyoruz
        float horizontalInput = inputActions.Player.Move.ReadValue<Vector2>().x;

        // Wall Jump sırasında kontrolü kısıtla
        if (wallJumpTimer > 0)
        {
            horizontalInput = 0f;
        }

        // Hareketi Uygula
        // Not: Unity 6 öncesi için 'rb.velocity', Unity 6 için 'rb.linearVelocity'
        Vector2 targetVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = targetVelocity;

        // Sprite Çevirme
        if (spriteRenderer != null && horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }

    // Zıplama inputu geldiğinde çalışacak fonksiyon
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // 1. Normal Zıplama
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
        // 2. Duvar Zıplaması (Wall Jump)
        else if (isTouchingWall && !isGrounded && wallJumpTimer <= 0)
        {
            wallJumpTimer = wallJumpCooldown;
            
            // Hızı sıfırla ki daha net bir zıplama olsun
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            Vector2 jumpVector = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
            rb.AddForce(jumpVector, ForceMode2D.Impulse);
        }
    }

    // Restart inputu geldiğinde çalışacak fonksiyon
    private void OnRestartPerformed(InputAction.CallbackContext context)
    {
        RestartGameInstant();
    }

    // --- Çarpışma ve Oyun Durumu Fonksiyonları (Değişmedi) ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bit shifting yerine LayerMask helper fonksiyonlarını kullanmak daha okunaklı olabilir
        // ama senin mevcut mantığını koruyorum:
        if (((1 << collision.gameObject.layer) & hunterLayer) != 0)
        {
            GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & goalLayer) != 0)
        {
            GameWin();
        }
    }

    void GameOver()
    {
        if (loseTextUI != null) loseTextUI.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(RestartGameAfterDelay(2f));
    }

    void GameWin()
    {
        if (winTextUI != null) winTextUI.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(RestartGameAfterDelay(4f));
    }

    void RestartGameInstant()
    {
        if (loseTextUI != null) loseTextUI.SetActive(false);
        if (winTextUI != null) winTextUI.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator RestartGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}