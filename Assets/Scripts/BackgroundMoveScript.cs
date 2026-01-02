using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect; // 1 = Gökyüzü (Kamerayla gider), 0 = Zemin (Normal durur)

    void Start()
    {
        startpos = transform.position.x;
        // Resmin genişliğini otomatik alıyoruz (Sprite Renderer şart!)
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Sonsuz döngü için kameranın ne kadar gittiğini hesapla (temp)
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        
        // Parallax efekti için objenin ne kadar gideceğini hesapla (dist)
        float dist = (cam.transform.position.x * parallaxEffect);

        // Objeyi yeni pozisyonuna taşı (Y eksenini sabit tutuyoruz ki aşağı kaymasın)
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // --- SONSUZ DÖNGÜ MANTIĞI ---
        // Eğer kamera resmin boyunu aştıysa, resmi ileri ışınla (Loop)
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}