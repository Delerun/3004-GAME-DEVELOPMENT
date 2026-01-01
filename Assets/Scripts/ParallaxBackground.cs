using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect; // 0 = Hareket etmez, 1 = Kamerayla aynı hızda gider

    void Start()
    {
        startpos = transform.position.x;
        // Resmin genişliğini alıyoruz (Sonsuz döngü için gerekli)
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Sonsuz döngü için kameranın ne kadar ilerlediğini hesapla
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        
        // Parallax efekti için asıl gidilecek mesafe
        float dist = (cam.transform.position.x * parallaxEffect);

        // Arka planı yeni pozisyona taşı
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // Eğer kamera görselin sınırını geçtiyse, görseli ileri ışınla (Sonsuz döngü)
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}