using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // A quién seguir
    public Vector3 offset = new Vector3(0, 2f, -10f); // Distancia

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}