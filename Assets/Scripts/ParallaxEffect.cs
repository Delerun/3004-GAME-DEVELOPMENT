using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public float parallaxFactor; // 0 = Sabit, 1 = Tam takip

    [Header("ÖNEMLİ: Resmin Genişliğini Buraya Elle Yaz")]
    public float manualLength; // Bunu Inspector'dan gireceksin!

    private float startpos;

    void Start()
    {
        startpos = transform.position.x;
    }

    void LateUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxFactor));
        float dist = (cam.transform.position.x * parallaxFactor);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // Sonsuz döngü (Girilen genişlik değerine göre çalışır)
        if (temp > startpos + manualLength) startpos += manualLength;
        else if (temp < startpos - manualLength) startpos -= manualLength;
    }
}