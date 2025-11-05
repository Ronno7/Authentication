using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody rig;
    private float startTime;
    private float timeTaken;
    private int collectablesPicked;
    public int maxCollectables = 10;
    private bool isPlaying;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;
        rig.linearVelocity = new Vector3(x, rig.linearVelocity.y, z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            collectablesPicked++;
            Destroy(other.gameObject);
            if (collectablesPicked == maxCollectables)
                End();
        }
    }
    public void Begin()
    {
        startTime = Time.time;
        isPlaying = true;
    }

    void End()
    {
        timeTaken = Time.time - startTime;
        isPlaying = false;
    }
}
