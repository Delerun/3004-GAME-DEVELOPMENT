using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Karakteri (Player) buraya sürükleyeceksin
    public float smoothSpeed = 0.125f; // Kameranın yumuşak gelme hızı (0-1 arası)
    public Vector3 offset; // Kamera ile karakter arasındaki mesafe farkı

    void LateUpdate() // LateUpdate ÇOK ÖNEMLİ! Karakter hareketini bitirdikten sonra kamera gelir.
    {
        if (target != null)
        {
            // Sadece X ve Y ekseninde takip et, Z ekseni (derinlik) bozulmasın
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            
            // Kamerayı o noktaya yumuşakça kaydır (Lerp)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}