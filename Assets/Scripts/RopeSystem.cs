using UnityEngine;
using System.Collections;

public class RopeSystem : MonoBehaviour
{
    [Header("HIZ AYARLARI")]
    public float swingSpeed = 50f;      // Sallanma hızı
    public float jumpVelocity = 15f;    // Zıplama Hızı (Force değil, net hız)
    
    [Header("BİLEŞENLER")]
    public HingeJoint2D playerJoint;    
    public Rigidbody2D rb;
    private SpriteRenderer sr; // Renk değişimi için (Debug)

    // Durumlar
    private bool isAttached = false;    
    private bool canGrab = true;        

    void Start()
    {
        // Bileşenleri otomatik bul
        if (playerJoint == null) playerJoint = GetComponent<HingeJoint2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // Görseli al (Varsa)

        // Başlangıç ayarı
        playerJoint.enabled = false;
    }

    void Update()
    {
        // Tuş kontrolünü en başa koydum.
        // Hem SPACE tuşunu hem de MOUSE SOL TIK'ı dinler.
        if (isAttached)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Debug.Log("KOMUT ALINDI: Zıplanıyor!"); // Konsolda bunu görmelisin
                DetachAndJump();
            }

            // SALLANMA
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput != 0)
            {
                rb.AddForce(Vector2.right * horizontalInput * swingSpeed * Time.deltaTime * 100f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // İpe ilk değme anı
        if (!isAttached && canGrab && collision.gameObject.CompareTag("Rope"))
        {
            AttachToRope(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }

    void AttachToRope(Rigidbody2D ropeSegment)
    {
        isAttached = true;
        playerJoint.connectedBody = ropeSegment;
        playerJoint.anchor = Vector2.zero;
        playerJoint.connectedAnchor = Vector2.zero;
        playerJoint.enabled = true;
        
        Debug.Log("TUTUNDU!"); // Konsolda bunu görüyordun
        
        // Görsel Test: Rengi KIRMIZI yap
        if(sr != null) sr.color = Color.red;
    }

    void DetachAndJump()
    {
        // 1. BAĞLANTIYI KES
        isAttached = false;
        playerJoint.enabled = false;
        playerJoint.connectedBody = null;
        
        // Görsel Test: Rengi YEŞİL yap
        if(sr != null) sr.color = Color.green;

        // 2. FİZİĞİ UYANDIR
        rb.WakeUp();

        // 3. YÖN HESAPLA
        float horizontalDir = Input.GetAxis("Horizontal");
        if (horizontalDir == 0) horizontalDir = transform.localScale.x; 
        if (horizontalDir == 0) horizontalDir = 1; // Sağa varsayılan

        // Yön Vektörü (Hafif yukarı)
        Vector2 jumpVector = new Vector2(horizontalDir, 0.8f).normalized;

        // 4. MİNİK IŞINLANMA (Yapışmayı önlemek için şart)
        transform.position += (Vector3)(jumpVector * 0.5f);

        // 5. HIZI DİREKT VER (Force değil Velocity)
        rb.linearVelocity = jumpVector * jumpVelocity; 

        // 6. COOLDOWN
        StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer()
    {
        canGrab = false;
        yield return new WaitForSeconds(0.5f);
        canGrab = true;
        // Rengi normale döndür
        if(sr != null) sr.color = Color.white;
    }
}