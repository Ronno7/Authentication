using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    public float amplitude = 0.2f;   // how far it moves up/down
    public float speed = 2f;         // oscillation speed
    public float rotationSpeed = 45f; // degrees per second, Z axis

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        // Up/down motion
        float newY = startY + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Z-axis rotation
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
