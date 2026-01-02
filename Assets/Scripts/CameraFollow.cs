using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Player'ı buraya sürükle
    public float smoothSpeed = 0.125f; 
    public Vector3 offset;         // Genelde (0, 0, -10) veya (2, 1, -10) iyidir

    [Header("Sınır Ayarları")]
    public bool limitAlt = true;   // Alt sınırı açmak için tikle
    public float minY = 0f;        // Kamera Y ekseninde bundan aşağı inemesin!

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Hedef pozisyonu hesapla
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            
            // 2. Yumuşak geçiş yap
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // ---[ İŞTE SİHİRLİ DOKUNUŞ BURASI ]---
            // Eğer "limitAlt" açıksa, Kameranın Y pozisyonunu "minY" değerine kilitler.
            if (limitAlt)
            {
                // Mathf.Max: "Hesaplanan Y mi büyük, yoksa minY mi? Büyük olanı al."
                // Böylece kamera minY'den daha aşağıya asla inemez.
                smoothedPosition.y = Mathf.Max(smoothedPosition.y, minY);
            }
            // -------------------------------------

            transform.position = smoothedPosition;
        }
    }
}