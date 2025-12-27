using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Zıplatma Gücü")]
    public float bounceForce = 20f; // Ne kadar yükseğe fırlatacak?

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarpan obje "Player" etiketine sahip mi kontrol et
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // ÖNEMLİ: Önce oyuncunun mevcut dikey hızını sıfırlıyoruz.
                // Bunu yapmazsak, yüksekten düşünce az, alçaktan düşünce çok zıplar.
                // Her seferinde aynı yükseğe zıplaması için sıfırlamak şart.
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                // Yukarı doğru ani güç uygula
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                
                // (Opsiyonel) Konsola bilgi ver
                Debug.Log("Tramboline basıldı!");
            }
        }
    }
}