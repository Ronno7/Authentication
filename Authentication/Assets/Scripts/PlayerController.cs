using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody rig;

    private float startTime;
    private float timeTaken;

    private int collectablesPicked;
    public int maxCollectables = 10;

    private bool isPlaying;

    public GameObject playButton;
    public TextMeshProUGUI curTimeText;

    void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Do nothing if the run is not active
        if (!isPlaying)
            return;

        // Movement
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;
        rig.linearVelocity = new Vector3(x, rig.linearVelocity.y, z);

        // Timer text
        curTimeText.text = (Time.time - startTime).ToString("F2");
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore triggers while not playing
        if (!isPlaying)
            return;

        if (other.CompareTag("Collectable"))
        {
            collectablesPicked++;
            Destroy(other.gameObject);

            Debug.Log($"Picked {collectablesPicked}/{maxCollectables}");

            if (collectablesPicked >= maxCollectables)
            {
                End();
            }
        }
    }

    public void Begin()
    {
        collectablesPicked = 0;
        startTime = Time.time;
        isPlaying = true;
        playButton.SetActive(false);
    }

    void End()
    {
        isPlaying = false;
        timeTaken = Time.time - startTime;

        playButton.SetActive(true);

        // Submit to leaderboard
        Leaderboard.instance.SetLeaderboardEntry(
            -Mathf.RoundToInt(timeTaken * 1000.0f)
        );
    }
}
